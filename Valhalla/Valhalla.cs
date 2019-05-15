using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valhalla
{
    class Valhalla
    {
        //static Random r = new System.Random();
        static CryptoRandom r = new CryptoRandom();

        public static List<Person> people = new List<Person>();
        public static List<Residence> residences = new List<Residence>();
        public static List<Occupation> occupations = new List<Occupation>();

        public static string[] maleNames = File.ReadAllLines("malenames.txt");
        public static string[] femaleNames = File.ReadAllLines("femalenames.txt");

        static void Main(string[] args)
        {
            int iterations;
            int food = 0;

            try
            {
                iterations = Convert.ToInt32(args[0]);
            } catch
            {
                Console.WriteLine("Argument error: " + args[0]);
                return;
            }
            
            Initialize();

            List<Person> newPeople = new List<Person>();

            // End of year cycle
            for (int i = 0; i < iterations; i++)
            {
                newPeople.Clear();

                Console.WriteLine("ITERATION " + i);
                Console.WriteLine("Number of people: " + people.Count(item => item.alive) + "\n");
                
                foreach (Person p in people)
                {
                    //Console.WriteLine(p);
                    if (p.alive)
                    {
                        p.age += 1;

                        // Death
                        float denom = (float) (0.001 * Math.Pow(10, p.age / 50.0f) - 0.001);
                        if (r.NextDouble() > (1.0f - denom))
                        {
                            p.Die();
                            Console.WriteLine(p + " has died");
                            continue;
                        }

                        // Get a job
                        if (p.occupation == null && p.age > 20)
                        {
                            p.occupation = r.Next(2) == 0 ? occupations.Find(item => item.name == "builder") : occupations.Find(item => item.name == "farmer");
                        }

                        // Marriage
                        if (p.spouse == null && p.age > 20)
                        {
                            //Console.WriteLine(p + " wants to get married");
                            foreach (Person x in people)
                            {
                                if (p.gender != x.gender && x.age > 20 && x.spouse == null && p.residence != x.residence && r.Next(3) == 0)
                                {
                                    p.Marry(x);
                                    Console.WriteLine(p + " and " + x + " got married!");
                                    break;
                                }
                            }
                        }

                        // Moving out
                        if (p.spouse != null && p.age > 20 && !p.residence.residents.Contains(p.spouse) && 
                            (p.residence.residents.Contains(p.parents[0]) || p.residence.residents.Contains(p.parents[1])))
                        {
                            Residence res = residences.Find(item => item.residents.Count == 0);
                            if (res != null)
                            {
                                p.residence.MoveOut(p);
                                p.spouse.residence.MoveOut(p.spouse);
                                res.MoveIn(p);
                                res.MoveIn(p.spouse);
                                Console.WriteLine(p + " and " + p.spouse + " moved in together!");
                            }
                            
                        }

                        // Occupation-based changes
                        if (p.occupation != null)
                        {
                            p.netWorth += p.occupation.salary;

                            // New residences
                            if (p.occupation.name == "builder" && r.Next(50) == 0)
                            {
                                if (r.Next(3) == 0)
                                {
                                    // House
                                    residences.Add(new Residence("house", r.Next(4, 7), r.Next(100000, 1000000)));
                                    Console.WriteLine(p + " built a new house!");

                                }
                                else
                                {
                                    // Apartment
                                    residences.Add(new Residence("apartment", r.Next(2, 5), r.Next(50000, 500000)));
                                    Console.WriteLine(p + " built a new apartment!");

                                }
                            }
                        } // occupation end

                        // Having a child
                        if (p.spouse != null && p.spouse.alive)
                        {
                            if (!p.residence.isFull() &&
                            p.residence.residents.Contains(p.spouse) &&
                            (p.gender == "F" && p.age > 20 && p.age < 50) || (p.gender == "M" && p.spouse.age > 20 && p.spouse.age < 50))
                            {
                                // Child percentage scales with age of mother, number of existing children
                                int motherAge = p.gender == "F" ? p.age : p.spouse.age;
                                //float prob = (float)(0.001 * Math.Pow(10, (-motherAge + 100) / 50.0f) - 0.001) / (p.children.Count + 1);
                                double prob = 2.5*(0.001 * Math.Pow(10, (-motherAge + 100) / 50.0f) - 0.001);

                                if (r.NextDouble() < prob)
                                {
                                    newPeople.Add(Birth(p, p.spouse));
                                    Console.WriteLine(p + " and " + p.spouse + " have a child " + newPeople.Last() + "!");
                                }

                            }
                        } // child end
                        
                    }
                    
                }

                people.AddRange(newPeople);

                Console.WriteLine();
                
            }

            // Do not remove
            Console.ReadLine();
            
        }

        static void Initialize()
        {
            // Initialize occupations
            using (StreamReader sr = new StreamReader("occupations.csv"))
            {
                string headerLine = sr.ReadLine();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] linesplit = line.Split(',');
                    occupations.Add(new Occupation(linesplit[0].Trim(), linesplit[1].Trim(), Convert.ToInt32(linesplit[2])));
                }
            }

            // Initialize 50 houses
            Person m, f;
            for (int i = 0; i < 50; i++)
            {
                residences.Add(new Residence("house", 6, 0));

                m = new Person(null, null, maleNames[r.Next(maleNames.Length)]);
                m.age = r.Next(20, 40);
                m.gender = "M";
                m.netWorth = 100000;
                m.occupation = occupations.Find(item => item.name == "builder");
                people.Add(m);

                residences[i].MoveIn(m);

                f = new Person(null, null, femaleNames[r.Next(femaleNames.Length)]);
                f.age = r.Next(20, 40);
                f.gender = "F";
                f.netWorth = 100000;
                f.occupation = occupations.Find(item => item.name == "farmer");
                people.Add(f);

                residences[i].MoveIn(f);

                m.Marry(f);
            }
            
        }

        public static Person Birth(Person father, Person mother)
        {

            Person p = new Person(father, mother, maleNames[r.Next(maleNames.Length)] + '/' + femaleNames[r.Next(femaleNames.Length)]);
            
            father.children.Add(p);
            mother.children.Add(p);

            return p;

        }
        
    }
}
