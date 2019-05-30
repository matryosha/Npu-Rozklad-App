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
            LessonUpdateType updateType)
        {
            OldLesson = oldLesson;
            NewLesson = newLesson;
            UpdateType = updateType;
        }

        public LessonUpdateType UpdateType { get; set; }

        public int LessonNumber =>
            UpdateType == LessonUpdateType.DeletedLesson
                ? OldLesson.LessonNumber
                : NewLesson.LessonNumber;


        public Fraction Fraction =>
            UpdateType == LessonUpdateType.DeletedLesson
                ? OldLesson.Fraction
                : NewLesson.Fraction;

        public SubGroup SubGroup =>
            UpdateType == LessonUpdateType.DeletedLesson
                ? OldLesson.SubGroup
                : NewLesson.SubGroup;

        public DateTime LessonDate =>
            UpdateType == LessonUpdateType.DeletedLesson
                ? OldLesson.LessonDate
                : NewLesson.LessonDate;

        public Lesson OldLesson { get; }
        public Lesson NewLesson { get; }
    }
}