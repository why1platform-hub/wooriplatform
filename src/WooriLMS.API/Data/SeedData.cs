using Microsoft.AspNetCore.Identity;
using WooriLMS.API.Models;

namespace WooriLMS.API.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Create roles
        string[] roles = { "Admin", "Instructor", "Normal" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Create admin user
        var adminEmail = "admin@woorilms.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Administrator",
                EmailConfirmed = true,
                UserType = UserType.Admin,
                IsActive = true,
                DateOfBirth = new DateTime(1980, 1, 1)
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Create sample instructor
        var instructorEmail = "instructor@woorilms.com";
        var instructorUser = await userManager.FindByEmailAsync(instructorEmail);

        if (instructorUser == null)
        {
            instructorUser = new ApplicationUser
            {
                UserName = instructorEmail,
                Email = instructorEmail,
                FirstName = "Jane",
                LastName = "Instructor",
                EmailConfirmed = true,
                UserType = UserType.Instructor,
                IsActive = true,
                DateOfBirth = new DateTime(1975, 6, 15),
                Bio = "Experienced career coach with 20+ years in corporate training."
            };

            var result = await userManager.CreateAsync(instructorUser, "Instructor@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(instructorUser, "Instructor");
            }
        }

        // Seed FAQs
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        if (!context.FAQs.Any())
        {
            var faqs = new List<FAQ>
            {
                new FAQ
                {
                    Question = "How do I enroll in a course?",
                    Answer = "Navigate to the Courses page, find a course you're interested in, and click the 'Enroll' button. You'll be able to start learning immediately.",
                    Category = "Courses",
                    OrderIndex = 1
                },
                new FAQ
                {
                    Question = "How do I book a consultant session?",
                    Answer = "Go to the Consultants page, browse available instructors, and select an available time slot. Submit your booking request with a brief description of what you'd like to discuss.",
                    Category = "Consultations",
                    OrderIndex = 2
                },
                new FAQ
                {
                    Question = "How do I update my profile?",
                    Answer = "Click on your profile icon in the top right corner and select 'My Profile'. From there, you can edit your personal information, skills, work experience, and upload your resume.",
                    Category = "Account",
                    OrderIndex = 3
                },
                new FAQ
                {
                    Question = "How do I apply for a program?",
                    Answer = "Visit the Programs page to see available skill development programs. Click on a program to view details and submit your application with a cover letter.",
                    Category = "Programs",
                    OrderIndex = 4
                },
                new FAQ
                {
                    Question = "How can I contact support?",
                    Answer = "You can post questions in the Discussion forum, send an email to support@woorilms.com, or check the FAQ section for common answers.",
                    Category = "Support",
                    OrderIndex = 5
                }
            };

            context.FAQs.AddRange(faqs);
            await context.SaveChangesAsync();
        }
    }
}
