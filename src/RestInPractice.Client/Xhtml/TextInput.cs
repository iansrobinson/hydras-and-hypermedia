using System;

namespace RestInPractice.Client.Xhtml
{
    public class TextInput
    {
        private readonly string name;

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

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (!string.IsNullOrEmpty(value) && String.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Value cannot be whitespace.", "value");
            }
            
            this.name = name;
            Value = value;
        }

        public string Name
        {
            get { return name; }
        }

        public string Value { get; set; }
    }
}