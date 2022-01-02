using System;
using System.Linq;
using System.Diagnostics;

namespace Ecosysteme
{
    abstract class Animal : Organism    //herbivores and carnivores
    {
        //ATTRIBUTES
        protected int sex;    //0=male, 1=female
        protected int visionRadius;
        protected int contactRadius;  //how close an animal has to be with an object to interact with it (eat, mate...)
        protected int[] direction;
        protected int walkSpeed;
        protected int runSpeed;   //used in case of hunting or fleeing
        protected bool pregnant;
        protected int pregnantTime;
        protected int gestationPeriod;
        //CONSTRUCTOR
        public Animal(int[] coordinates) :
        base(coordinates)
        {
            Random rnd = new Random();
            sex = rnd.Next(2);
            direction = new[] { rnd.Next(-1, 2), rnd.Next(-1, 2) }; //random cardinal direction (examples: (-1, 1)=NW ; (1,0)=E ; (1,-1)=SE)
            while (direction[0] == 0 && direction[1] == 0)    //(0,0) is not a direction, so we generate a new one
            {
                direction[0] = rnd.Next(-1, 2);
                direction[1] = rnd.Next(-1, 2);
            }
            pregnant = false;
            pregnantTime = 0;
        }
        //METHODS
        public override void Iterate(Entities entities)
        {
            //losing/regenerating of energy/life
            base.Iterate(entities);
            if (entities.getList().Contains(this))  //not dead yet
            {
                //pooping
                if (entities.Chance(10)) { this.Poop(entities); }
                //pregnancy
                if (pregnant) { this.PregnancyIteration(entities); }
                //actions (feeding / mating)
                this.Action(entities);
            }
        }
        private void Move(int speed)  //moves the animal in the habitat (distance=f(speed))
        {
            if (direction.Sum() == 1 || direction.Sum() == -1)
            {
                coordinates[0] += direction[0] * speed;
                coordinates[1] += direction[1] * speed;
            }
            else
            {
                coordinates[0] += (int)Math.Round(direction[0] * speed * 0.7);
                coordinates[1] += (int)Math.Ceiling(direction[1] * speed * 0.7);
            }
        }
        public void Walk() { this.Move(walkSpeed); }
        public void Run() { this.Move(runSpeed); }
        private void ChangeDirection(int[] direction) { this.direction = direction; }
        private void RandomDirection(Entities entities)
        {
            direction = new[] { entities.random.Next(-1, 2), entities.random.Next(-1, 2) }; //random cardinal direction (examples: (-1, 1)=NW ; (1,0)=E ; (1,-1)=SE)
            while (direction[0] == 0 && direction[1] == 0)    //(0,0) is not a direction, so we generate a new one
            {
                direction[0] = entities.random.Next(-1, 2);
                direction[1] = entities.random.Next(-1, 2);
            }
        }
        private void Poop(Entities entities) { entities.Add(new OrganicWaste(coordinates, energy / 5)); }    //when an animal poops, it leaves organic waste behind (which can be consumed by plants)
        private void PregnancyIteration(Entities entities)
        {
            if (pregnantTime == gestationPeriod)
            {
                pregnant = false;
                pregnantTime = 0;
                entities.Add(this.Reproduce(coordinates));
            }
            else { pregnantTime++; }
        }
        private void Action(Entities entities)
        {
            Entity food = FindFood(entities);
            if ((food == null || energy > 80 || (energy > 10 && life > 50)) && !pregnant)   //food is not a priority / there is no food
            {
                this.NotFoodIteration(entities, food);
            }
            else    //food is a priority / already pregnant
            {
                this.FoodIteration(entities, food);
            }
        }
        protected abstract Entity FindFood(Entities entities);
        private void FoodIteration(Entities entities, Entity food)
        {
            if (food == null)
            {
                if (entities.Chance(10)) { this.RandomDirection(entities); }    //animal changes direction regularly so it doesn't wander off too far away from the other entities
                this.Walk();
            }
            else if (Coordinates.Distance(coordinates, food.getCoordinates()) > contactRadius)   //walk or run towards food
            {
                int distance = Coordinates.Distance(coordinates, food.getCoordinates());
                this.ChangeDirection(Coordinates.Direction(coordinates, food.getCoordinates()));
                if (food is Herbivore)  //hunting
                {
                    if (distance < runSpeed) { coordinates = food.getCoordinates(); }
                    else if (distance < 50) { this.Run(); }     //if the animal is close, it runs
                    else { this.Walk(); }
                }
                else { this.Walk(); }
            }
            else if (!(food is Animal))  //eat food
            {
                if (food.GetType() == typeof(Meat))
                {
                    Meat meat = (Meat)food;
                    this.EatMeat(entities, meat);
                }
                else
                {
                    Plant plant = (Plant)food;
                    this.EatPlant(entities, plant);
                }
            }
            else  //kill food
            {
                Animal prey = (Animal)food;
                prey.Damage(25);
            }
        }
        private void EatPlant(Entities entities, Plant plant)
        {
            int emptyEnergy = 100 - energy;
            int life = plant.getLife();
            int calorieDensity = plant.getCalorieDensity();
            while (emptyEnergy > 0 && life > 0)
            {
                emptyEnergy -= calorieDensity;
                energy += calorieDensity;
                if (life >= 10 / plant.getOccupiedArea().Count) { life -= 10 / plant.getOccupiedArea().Count; }
                else { life = 0; }
            }
            if (life == 0)
            {
                entities.Remove(plant);
            }
            else
            {
                plant.Leave(life);
            }
        }
        private void Damage(int amount)
        {
            if (life < amount) { life = 0; }
            else { life -= amount; }
        }
        private void EatMeat(Entities entities, Meat meat)
        {
            int emptyEnergy = 100 - energy;
            int calories = meat.getCalories();
            while (emptyEnergy > 0 && calories > 0)
            {
                emptyEnergy--;
                energy++;
                calories--;
            }
            if (calories == 0)
            {
                entities.Remove(meat);
            }
            else
            {
                meat.Leave(calories);
            }
        }
        private void NotFoodIteration(Entities entities, Entity food)
        {
            Animal mate = FindMate(entities);
            if (mate == null)   //no mate in sight
            {
                if (food != null) { this.FoodIteration(entities, food); }   //food is not a priority, but there is nothing else to do
                else  //there is nothing of interest (no food or mate)
                {
                    if (entities.Chance(10)) { this.RandomDirection(entities); }    //animal changes direction regularly so it doesn't wander off too far away from the other entities
                    this.Walk();
                }
            }
            else if (Coordinates.Distance(coordinates, mate.getCoordinates()) <= contactRadius) //mate
            {
                this.Mate();
                mate.Mate();
            }
            else  //go towards mate
            {
                this.ChangeDirection(Coordinates.Direction(coordinates, mate.getCoordinates()));
                this.Walk();
            }
        }
        private Animal FindMate(Entities entities)   //finds the closest eligible mate within the vision radius
        {
            Animal response = null;
            int distance = 10000;
            foreach (Entity entity in entities.getList())
            {
                if (entity.GetType() == this.GetType() && Coordinates.Distance(coordinates, entity.getCoordinates()) < visionRadius && Coordinates.Distance(coordinates, entity.getCoordinates()) < distance)
                {
                    if (sex != ((Animal)entity).getSex() && !((Animal)entity).getPregnant())  //not same sex and not yet pregnant
                    {
                        response = (Animal)entity;
                        distance = Coordinates.Distance(coordinates, entity.getCoordinates());
                    }
                }
            }
            return response;
        }
        private void Mate() { if (this.getSex() == 1) { pregnant = true; } }  //female -> pregnancy starts
        //ACCESSORS
        public int getSex() { return sex; }
        public int getVisionRadius() { return visionRadius; }
        public int getContactRadius() { return contactRadius; }
        public int[] getDirection() { return direction; }
        public int getWalkSpeed() { return walkSpeed; }
        public int getRunSpeed() { return runSpeed; }
        public bool getPregnant() { return pregnant; }
    }
}
