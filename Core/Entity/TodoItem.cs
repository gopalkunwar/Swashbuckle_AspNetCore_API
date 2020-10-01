using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Core.Entity
{
    /// <summary>
    /// A TodoItem tracks a task
    /// </summary>
    public class TodoItem
    {
        /// <summary>
        /// The Id of the TodoItem
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }
        /// <summary>
        /// The task name
        /// </summary>
        /// <example>Car Maintenance</example>
        public string Title { get; set; }
        /// <summary>
        /// The task is completed or not. Default:<code>false</code>
        /// </summary>
        /// <example>false</example>
        public bool Completed { get; set; }
    }
}
