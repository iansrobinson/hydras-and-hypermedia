using System;

namespace RestInPractice.Server.Xhtml
{
    public class TextInput
    {
        private readonly string name;
        private readonly string value;

        public TextInput(string name) : this(name, null)
        {
        }

        public TextInput(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Value cannot be empty or whitespace.", "name");
            }

            this.name = name;
            this.value = value;
        }

        public string Name
        {
            get { return name; }
        }

        public string Value
        {
            get { return value; }
        }
    }
}