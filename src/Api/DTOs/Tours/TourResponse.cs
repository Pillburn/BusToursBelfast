public record TourResponse(
    string Id,
    string Name,
    string Description,
    decimal Price,
    string? ImageUrl = null) // Make optional with default value
{
    // This will serialize to JSON as:
    // {
    //   "Id": "string",
    //   "Name": "string", 
    //   "Description": "string",
    //   "Price": "decimal",
    //  "ImageUrl": "string?"
    // }
};