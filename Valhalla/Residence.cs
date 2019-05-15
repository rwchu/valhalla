using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valhalla
{
    class Residence
    {
        public string[] residenceTypes = new string[] { "house", "apartment" };
        public List<Person> residents = new List<Person>();

        public int capacity, value;
        public string type;

        public Residence(string type, int capacity, int value)
        {
            if (!residenceTypes.Any(x => x == type))
                throw new ArgumentException("Not valid residential type");
            else
                this.type = type;
            
            this.capacity = capacity;
            this.value = value;
        }

        public void MoveIn(Person person)
        {
            if (residents.Count < capacity)
            {
                residents.Add(person);
                person.residence = this;
            }
                
            
        }

        public void MoveOut(Person person)
        {
            if (residents.Contains(person))
            {
                residents.Remove(person);
                person.residence = null;
            }
                
            
        }

        public bool isFull()
        {
            return residents.Count == capacity;
        }
        

    }
}
