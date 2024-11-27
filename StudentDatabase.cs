using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace EX_11_2
{
    public class StudentDatabase
    {
        private string _curriculumAbbreviation;
        private readonly Dictionary<string, string> _students; // Key: student code, Value: student name

        public StudentDatabase(string curriculumAbbreviation)
        {
            // Validate curriculum abbreviation
            _curriculumAbbreviation = ValidateCurriculumAbbreviation(curriculumAbbreviation) ? curriculumAbbreviation : "IABB";
            _students = new Dictionary<string, string>();
        }

        private bool ValidateCurriculumAbbreviation(string abbreviation)
        {
            // Ensure the abbreviation is not null or empty, has length 4, and contains only letters.
            return !string.IsNullOrEmpty(abbreviation) && abbreviation.Length == 4 && abbreviation.All(char.IsLetter);
        }

        public Dictionary<string, string> GetStudents()
        {
            return _students;
        }

        public string GenerateStudentCode()
        { // Create a new Random object to generate random numbers.
            Random random = new Random();
            string code;
            do
            {
                code = _curriculumAbbreviation + random.Next(100000, 999999).ToString();
                // Check if the generated code already exists in the dictionary. If it does, generate a new code.
            } while (_students.ContainsKey(code)); // To avoid duplicates

            return code;
        }

        public void AddStudent(string studentName)
        {
            if (string.IsNullOrEmpty(studentName)) throw new ArgumentException("Student name cannot be empty.");

            string uniqueName = GetUniqueName(studentName);
            string studentCode = GenerateStudentCode();

            _students.Add(studentCode, uniqueName);
        }

        public void AddStudent(string studentCode, string studentName)
        {
            // Ensure the provided student code starts with the curriculum abbreviation.
            // The comparison is case-insensitive to handle different casing scenarios.
            if (string.IsNullOrEmpty(studentName)) throw new ArgumentException("Student name cannot be empty.");
            if (!studentCode.StartsWith(_curriculumAbbreviation, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Student code must contain the curriculum abbreviation {_curriculumAbbreviation}. Value not added.");
                return;
            }

            string uniqueName = GetUniqueName(studentName);

            // Check if the student code already exists in the dictionary.
            if (!_students.ContainsKey(studentCode))
            {
                // If the code is unique, add the student code and the unique name to the dictionary.
                _students.Add(studentCode, uniqueName);
            }
            else
            {
                // If the code already exists in the dictionary, print an error message and exit.
                Console.WriteLine($"Student code must be unique. Value not added.");
                return;
            }
        }

        private string GetUniqueName(string studentName)
        {
            string uniqueName = studentName;
            int counter = 1;

            // Check if the current name already exists in the dictionary's values.
            // If it does, append a number to the name and increment the counter.
            while (_students.ContainsValue(uniqueName))
            {
                // Generate a new candidate name by appending the counter to the original name
                uniqueName = $"{studentName} {counter}";
                // Increment the counter for the next iteration.
                counter++;
            }

            // Return the unique name that does not already exist in the dictionary.
            return uniqueName;
        }

        public void PrintNames(bool sorted)
        {
            Dictionary<string, string> studentsToPrint;

            if (sorted)
            {
                // If sorting is requested, order the students by their names (values) in ascending order.
                // Convert the ordered sequence back into a dictionary.
                studentsToPrint = _students.OrderBy(s => s.Value).ToDictionary(s => s.Key, s => s.Value);
            }
            else
            {
                // If no sorting is requested, use the existing student dictionary as is.
                studentsToPrint = _students;
            }

            // Iterate through the dictionary and print each student's code and name
            foreach (var student in studentsToPrint)
            {
                Console.WriteLine($"Code: {student.Key}, Name: {student.Value}");
            }
        }


        public void WriteToFile(string filePath)
        {
            // Sort the students by their names (values) in ascending order.
            var sortedStudents = _students.OrderBy(s => s.Value);

            // Open a file for writing using a StreamWriter, ensuring it is properly disposed of after use.
            using StreamWriter writer = new StreamWriter(filePath);

            // Write each student's code and name to the file in the sorted order.
            foreach (var student in sortedStudents)
            {
                writer.WriteLine($"Code: {student.Key}, Name: {student.Value}");
            }
        }

        public string GetStudentWithLongestName()
        {
            if (_students.Count == 0) return null;

            return _students.OrderByDescending(s => s.Value.Length).First().Value;
        }

        public static void Main(string[] args)
        {

        }
    }
}