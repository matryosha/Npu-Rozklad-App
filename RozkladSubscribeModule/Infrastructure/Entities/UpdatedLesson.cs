using System;
using NpuTimetableParser;
using RozkladSubscribeModule.Infrastructure.Enums;

namespace RozkladSubscribeModule.Infrastructure.Entities
{

    internal class UpdatedLesson
    {
        public UpdatedLesson(
            Lesson oldLesson, 
            Lesson newLesson,
            UpdatedLessonType updateType)
        {
            OldLesson = oldLesson;
            NewLesson = newLesson;
            UpdateType = updateType;
        }

        public UpdatedLessonType UpdateType { get; set; }

        public int LessonNumber =>
            UpdateType == UpdatedLessonType.DeletedLesson
                ? OldLesson.LessonNumber
                : NewLesson.LessonNumber;


        public Fraction Fraction =>
            UpdateType == UpdatedLessonType.DeletedLesson
                ? OldLesson.Fraction
                : NewLesson.Fraction;

        public SubGroup SubGroup =>
            UpdateType == UpdatedLessonType.DeletedLesson
                ? OldLesson.SubGroup
                : NewLesson.SubGroup;

        public DateTime LessonDate =>
            UpdateType == UpdatedLessonType.DeletedLesson
                ? OldLesson.LessonDate
                : NewLesson.LessonDate;

        public Lesson OldLesson { get; }
        public Lesson NewLesson { get; }
    }
}