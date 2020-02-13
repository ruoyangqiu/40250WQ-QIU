using System;

namespace Mine.Models
{
    /// <summary>
    /// Item for the Game
    /// </summary>
    public class ItemModel : BaseModel
    {
        // Add Unique attributes for Item

            // The Value of the item
        public int Value { get; set; } = 0;

        public bool Update(ItemModel data)
        {
            // Update Base
            Name = data.Name;
            Description = data.Description;

            // Update the extended
            Value = data.Value;

            return true;
        }
    }

    public class History
    {
        // Change DateTime
        public DateTime ChangeDateTime { get; set; } = DateTime.Now;
        // Comments about the Change
        public string Note { get; set; } = "Note";
        // The Latest Record
        public string ChangedLatest { get; set; } = string.Empty;
        // The Previous Record
        public string ChangedPrevious { get; set; } = string.Empty;
        // The Size of the change between Latest - Prevous
        public int ChangeSize { get; set; } = 0;
    }
}