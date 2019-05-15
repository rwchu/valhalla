using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valhalla
{
    class Occupation
    {
        Random r = new Random();

        public int salary;
        public string name, field;

        public Occupation(string name, string field, int salary)
        {
            this.name = name;
            this.field = field;
            this.salary = salary;
        }

        // Not person-specific yet
        /*public void Raise()
        {
            // Raise 3-5% of current annual salary
            this.salary += (int) Math.Round(r.NextDouble() * salary * 0.02 + salary * 0.03);
        }*/

        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}", name, field, salary);
        }
    }
}
