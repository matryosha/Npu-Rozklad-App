# NPU-Timetables-grabber
## How to use:
1. [Download](https://github.com/matryosha/NPU-Timetables-grabber/releases/download/v0.1-beta/NpuTimetableParser.dll) latest version
2. Place dll somewhere
3. [Add dependency to your project](https://stackoverflow.com/questions/7348675/how-do-i-put-a-dll-into-my-project-visual-studio-c-sharp-2010)
4. Add RestClient to your project
## Usage Examples:
### 1:
```c#
   //Create instance
   var npuParser = new NpuParser();
   
   //Get all faculties
   var faculties = await npuParser.GetFaculties();
   
   //Select certain faculty
   var fi = faculties.First(f => f.ShortName == "fi");
   
   //Set faculty sending faculty object
   npuParser.SetFaculty(fi);
   //Or just using short name
   npuParser.SetFaculty(fi.ShortName);
   
   //Get faculty's groups
   var groups = await npuParser.GetGroups();
   
   //Get lesson list by passing date and group id
   var lessons = await npuParser.GetLessonsOnDate(new DateTime(2018, 10, 29), groups[1].ExternalId); 
```
### 2:
```c#
   //Create instance wiht faculty name
   var npuParser = new NpuParser("fi");
   
   //Get faculty's groups
   var groups = await npuParser.GetGroups();
   
   //Get lesson list by passing date and group id
   var lessons = await npuParser.GetLessonsOnDate(new DateTime(2018, 10, 29), groups[1].ExternalId);   
```
### GetLessonsOnDate returns List of Lesson object:
```c#
    public class Lesson
    {
        public Group Group { get; set; }
        public Subject Subject { get; set; }
        public Classroom Classroom { get; set; }
        public Lecturer Lecturer { get; set; }
        public int LessonNumber { get; set; }
        public DateTime LessonDate { get; set; }
        public Fraction Fraction { get; set; }
        public SubGroup SubGroup { get; set; }
        public int LessonCount { get; set; }
        
        ...
    }
```
