using UnityEngine;

namespace MoodyBlues.Fire
{
    public class PersonFactory
    {
        private readonly GameObject person;

        public PersonFactory(GameObject person)
        {
            this.person = person;
        }

        public void CreatePerson(PersonParameters args)
        {
            GameObject p = Object.Instantiate(person);
            p.GetComponent<Person>().Initialize(args);
            p.GetComponent<Person>().StartPath();
        }
    }
}
