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
var faculties = npuParser.GetFaculties();

//Select certain faculty
var fi = faculties.First(f => f.ShortName == "fi");

//Get faculty's groups
var groups = await npuParser.GetGroups(fi.ShortName);

//Get classes
var classes = await npuParser.GetLessonsOnDate(
                 fi.ShortName, 
                 groups[4].ExternalId,
                 new DateTime(2019, 3, 18)); 
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
