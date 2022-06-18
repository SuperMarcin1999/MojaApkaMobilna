using System;
using System.Collections.Generic;
using System.Text;

namespace MojaApkaMobilna
{
    class Person
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public Person(){}
        public Person(string name, string password)
        {
            Name = name;
            Password = password;
        }
    }
}
