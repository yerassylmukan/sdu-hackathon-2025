using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1.Data;

public class ApplicationDbContextSeed
{
    public static async Task SeedDataAsync(ApplicationDbContext context)
    {
        if (context.Database.IsNpgsql()) context.Database.Migrate();

        if (!await context.Categories.AnyAsync())
        {
            var categories = GetPreconfiguredCategories();
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }
    }

    private static List<Category> GetPreconfiguredCategories()
    {
        var photoUrl =
            "https://www.allrecipes.com/thmb/5JVfA7MxfTUPfRerQMdF-nGKsLY=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/25473-the-perfect-basic-burger-DDMFS-4x3-56eaba3833fd4a26a82755bcd0be0c54.jpg";

        return new List<Category>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Burgers",
                Foods = new List<Food>
                {
                    CreateFood("Classic Cheeseburger", "Beef patty with cheddar cheese", photoUrl),
                    CreateFood("Bacon Burger", "Beef patty, bacon, lettuce, tomato", photoUrl),
                    CreateFood("Veggie Burger", "Plant-based patty, avocado", photoUrl),
                    CreateFood("Double Beef Burger", "Two patties, cheese, pickles", photoUrl),
                    CreateFood("Spicy Jalapeño Burger", "Beef patty, jalapeños, hot sauce", photoUrl)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Pizzas",
                Foods = new List<Food>
                {
                    CreateFood("Margherita", "Tomato, mozzarella, basil", photoUrl),
                    CreateFood("Pepperoni", "Classic pepperoni and cheese", photoUrl),
                    CreateFood("Hawaiian", "Ham and pineapple", photoUrl),
                    CreateFood("BBQ Chicken", "Chicken, barbecue sauce, onion", photoUrl),
                    CreateFood("Meat Lovers", "Pepperoni, sausage, ham, bacon", photoUrl)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Pastas",
                Foods = new List<Food>
                {
                    CreateFood("Spaghetti Bolognese", "Ground beef, tomato sauce", photoUrl),
                    CreateFood("Fettuccine Alfredo", "Creamy white sauce", photoUrl),
                    CreateFood("Penne Arrabbiata", "Spicy tomato sauce", photoUrl),
                    CreateFood("Carbonara", "Eggs, bacon, parmesan", photoUrl),
                    CreateFood("Lasagna", "Layered pasta with meat and cheese", photoUrl)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Salads",
                Foods = new List<Food>
                {
                    CreateFood("Caesar Salad", "Romaine, parmesan, croutons", photoUrl),
                    CreateFood("Greek Salad", "Cucumber, tomato, feta", photoUrl),
                    CreateFood("Cobb Salad", "Chicken, egg, bacon", photoUrl),
                    CreateFood("Caprese Salad", "Tomato, mozzarella, basil", photoUrl),
                    CreateFood("Quinoa Salad", "Quinoa, veggies, lemon dressing", photoUrl)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Desserts",
                Foods = new List<Food>
                {
                    CreateFood("Chocolate Cake", "Rich chocolate sponge", photoUrl),
                    CreateFood("Cheesecake", "Creamy vanilla cheesecake", photoUrl),
                    CreateFood("Apple Pie", "Classic apple and cinnamon", photoUrl),
                    CreateFood("Tiramisu", "Coffee-flavored Italian dessert", photoUrl),
                    CreateFood("Ice Cream Sundae", "Vanilla with toppings", photoUrl)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Sandwiches",
                Foods = new List<Food>
                {
                    CreateFood("Club Sandwich", "Turkey, bacon, lettuce, tomato", photoUrl),
                    CreateFood("BLT", "Bacon, lettuce, tomato", photoUrl),
                    CreateFood("Grilled Cheese", "Melted cheese on toast", photoUrl),
                    CreateFood("Tuna Melt", "Tuna salad and cheese", photoUrl),
                    CreateFood("Chicken Panini", "Grilled chicken and pesto", photoUrl)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Soups",
                Foods = new List<Food>
                {
                    CreateFood("Tomato Soup", "Creamy tomato base", photoUrl),
                    CreateFood("Chicken Noodle Soup", "Chicken, noodles, vegetables", photoUrl),
                    CreateFood("Minestrone", "Italian vegetable soup", photoUrl),
                    CreateFood("Clam Chowder", "Creamy seafood soup", photoUrl),
                    CreateFood("Lentil Soup", "Hearty lentils and spices", photoUrl)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Breakfast",
                Foods = new List<Food>
                {
                    CreateFood("Pancakes", "Fluffy pancakes with syrup", photoUrl),
                    CreateFood("Omelette", "Eggs with fillings", photoUrl),
                    CreateFood("Avocado Toast", "Sourdough with avocado", photoUrl),
                    CreateFood("French Toast", "Bread soaked in egg and fried", photoUrl),
                    CreateFood("Breakfast Burrito", "Eggs, sausage, cheese in a wrap", photoUrl)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Drinks",
                Foods = new List<Food>
                {
                    CreateFood("Iced Coffee", "Chilled coffee with milk", photoUrl),
                    CreateFood("Lemonade", "Fresh lemon juice", photoUrl),
                    CreateFood("Smoothie", "Blended fruits and yogurt", photoUrl),
                    CreateFood("Milkshake", "Ice cream blended with milk", photoUrl),
                    CreateFood("Iced Tea", "Chilled brewed tea", photoUrl)
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Asian Cuisine",
                Foods = new List<Food>
                {
                    CreateFood("Sushi Roll", "Rice, seaweed, fish", photoUrl),
                    CreateFood("Pad Thai", "Stir-fried noodles", photoUrl),
                    CreateFood("Sweet and Sour Chicken", "Fried chicken in sweet sauce", photoUrl),
                    CreateFood("Beef Teriyaki", "Grilled beef with teriyaki sauce", photoUrl),
                    CreateFood("Spring Rolls", "Vegetables wrapped in rice paper", photoUrl)
                }
            }
        };
    }

    private static Food CreateFood(string name, string description, string photoUrl)
    {
        return new Food
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            PhotoUrl = photoUrl,
            Price = 9.99m,
            PreparationTime = TimeSpan.FromMinutes(15),
            Recipe = new List<string> { "Step 1", "Step 2", "Step 3" }
        };
    }
}