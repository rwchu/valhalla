using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Valhalla
{
    class Person
    {
        Random r = new Random();
        CryptoRandom rng = new CryptoRandom();

        public int age;
        public float netWorth;
        public string name, gender;
        public bool alive;
        public Occupation occupation;
        public Residence residence;
        public Person spouse;
        public Person[] parents;
        public List<Person> children;
        
        // I CREATE LIFE
        public Person(Person parent1, Person parent2, string name)
        {
            // Initialize variables
            age = 0;
            netWorth = 0;
            alive = true;
            children = new List<Person>();

            // Assign random gender
            gender = rng.Next(2) == 0 ? "M" : "F";

            // Name
            if (name.Contains("/"))
                this.name = gender == "M" ? name.Split('/')[0] : name.Split('/')[1];
            else
                this.name = name;

            parents = new Person[2];
            parents[0] = parent1;
            parents[1] = parent2;

            if (parents[0] != null)
                residence = parents[0].residence;
            
        }

        // ...and i destroy it
        public void Die()
        {
            alive = false;
            residence.MoveOut(this);
        }

        public void Marry(Person person)
        {
            this.spouse = person;
            person.spouse = this;
        }

        public override string ToString()
        {

            // Name, gender, age, occupation, net worth
            return String.Format("{0}({1}{2})", name, age, gender);
        }
    }
}
