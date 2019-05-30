using System.Collections.Generic;
using System.Linq;
using NpuTimetableParser;
using RozkladSubscribeModule.Infrastructure.Entities;
using RozkladSubscribeModule.Infrastructure.Enums;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule.Entities
{
    internal class DefaultCheckPayload : ICheckPayload
    {
        public List<UpdatedLesson> UpdatedLessons = new List<UpdatedLesson>();

        public bool IsDiff() => UpdatedLessons.Any();

        public void AddNewLesson(Lesson newLesson)
        {
            UpdatedLessons.Add(
                new UpdatedLesson(null, newLesson, LessonUpdateType.AddedLesson));
        }

        public void AddDeletedLesson(Lesson deletedLesson)
        {
            UpdatedLessons.Add(
                new UpdatedLesson(deletedLesson, null, LessonUpdateType.DeletedLesson));
        }

        public void AddReplacedLesson(Lesson oldLesson, Lesson newLesson)
        {
            UpdatedLessons.Add(
                new UpdatedLesson(oldLesson, newLesson, LessonUpdateType.ReplacedLesson));
        }
    }
}