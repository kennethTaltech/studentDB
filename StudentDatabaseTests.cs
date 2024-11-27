using EX_11_2;


namespace EX_11_2_Tests
{
    [TestClass]
    public class StudentDatabaseTests
    {
        private StudentDatabase _db;

        [TestInitialize]
        public void SetUp()
        {
            _db = new StudentDatabase("ITEC");
        }

        [TestMethod]
        public void Constructor_ShouldUseValidCurriculumAbbreviation()
        {
            var db = new StudentDatabase("COMP");
            db.AddStudent("John Doe");
            Assert.AreEqual("COMP", db.GetStudents().Keys.FirstOrDefault()?.Substring(0, 4));
        }

        [TestMethod]
        public void Constructor_ShouldFallbackToDefaultAbbreviation_WhenInvalidProvided()
        {
            var db = new StudentDatabase("XYZ123");
            db.AddStudent("Student Name");
            Assert.IsTrue(db.GetStudents().Keys.FirstOrDefault().StartsWith("IABB"));
        }

        [TestMethod]
        public void AddStudent_ShouldGenerateUniqueCode_ForEachStudent()
        {
            // Act
            _db.AddStudent("Alice");
            _db.AddStudent("Bob");

            // Assert
            var students = _db.GetStudents();
            Assert.AreEqual(2, students.Count);
            Assert.IsTrue(students.Keys.ElementAt(0) != students.Keys.ElementAt(1)); // Unique codes
        }

        [TestMethod]
        public void AddStudent_ShouldHandleDuplicateNames_WithUniqueNameSuffix()
        {
            // Act
            _db.AddStudent("Alice");
            _db.AddStudent("Alice");

            // Assert
            var students = _db.GetStudents();
            Assert.AreEqual(2, students.Count);
            Assert.AreEqual("Alice", students.Values.ElementAt(0));
            Assert.AreEqual("Alice 1", students.Values.ElementAt(1)); // Duplicate resolved
        }

        [TestMethod]
        public void AddStudentWithCode_ShouldAddStudent_WhenCodeIsValid()
        {
            // Act
            _db.AddStudent("ITEC123456", "John Wick");

            // Assert
            var students = _db.GetStudents();
            Assert.AreEqual(1, students.Count);
            Assert.IsTrue(students.ContainsKey("ITEC123456"));
            Assert.AreEqual("John Wick", students["ITEC123456"]);
        }

        [TestMethod]
        public void AddStudentWithCode_ShouldRejectStudent_WhenCodeIsInvalid()
        {
            // Act
            _db.AddStudent("WRONG123456", "Invalid Student");

            // Assert
            var students = _db.GetStudents();
            Assert.AreEqual(0, students.Count); // No student should be added
        }

        [TestMethod]
        public void PrintNames_ShouldSortByName_WhenTrue()
        {
            // Arrange
            _db.AddStudent("Charlie");
            _db.AddStudent("Alice");
            _db.AddStudent("Bob");

            // Act
            using var sw = new StringWriter();
            Console.SetOut(sw);

            _db.PrintNames(true);
            var output = sw.ToString().Trim().Split("\n");

            // Assert
            Assert.IsTrue(output[0].Contains("Alice"));
            Assert.IsTrue(output[1].Contains("Bob"));
            Assert.IsTrue(output[2].Contains("Charlie"));
        }

        [TestMethod]
        public void PrintNames_ShouldNotSortByName_WhenFalse()
        {
            // Arrange
            _db.AddStudent("Charlie");
            _db.AddStudent("Alice");
            _db.AddStudent("Bob");

            // Act
            using var sw = new StringWriter();
            Console.SetOut(sw);

            _db.PrintNames(false);
            var output = sw.ToString().Trim().Split("\n");

            // Assert
            Assert.IsTrue(output[0].Contains("Charlie")); // Maintains insertion order
            Assert.IsTrue(output[1].Contains("Alice"));
            Assert.IsTrue(output[2].Contains("Bob"));
        }

        [TestMethod]
        public void GetStudentWithLongestName_ShouldReturnCorrectStudent()
        {
            // Arrange
            _db.AddStudent("Short Name");
            _db.AddStudent("A much longer student name");

            // Act
            string longestName = _db.GetStudentWithLongestName();

            // Assert
            Assert.AreEqual("A much longer student name", longestName);
        }

        [TestMethod]
        public void WriteToFile_ShouldWriteSortedStudentsToFile()
        {
            // Arrange
            string filePath = "test_students.txt";
            _db.AddStudent("Charlie");
            _db.AddStudent("Alice");
            _db.AddStudent("Bob");

            // Act
            _db.WriteToFile(filePath);

            // Assert
            var lines = File.ReadAllLines(filePath);
            Assert.AreEqual(3, lines.Length);
            Assert.IsTrue(lines[0].Contains("Alice"));
            Assert.IsTrue(lines[1].Contains("Bob"));
            Assert.IsTrue(lines[2].Contains("Charlie"));
        }
    }
}
