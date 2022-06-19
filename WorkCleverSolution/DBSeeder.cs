using WorkCleverSolution.Data;
using WorkCleverSolution.Data.Identity;
using WorkCleverSolution.Services;
using Microsoft.AspNetCore.Identity;
using WorkCleverSolution.Dto.Auth;
using WorkCleverSolution.Dto.Global;
using WorkCleverSolution.Dto.Project;
using WorkCleverSolution.Dto.Project.Board;
using WorkCleverSolution.Dto.Project.Column;
using WorkCleverSolution.Dto.User;

namespace WorkCleverSolution;

public static class DbSeeder
{
    public static async Task Seed(ApplicationDbContext dbContext, IServices appServices)
    {
        // dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        if (!dbContext.SiteSettings.Any())
        {
            await dbContext.SiteSettings.AddAsync(new SiteSettings
            {
                DefaultTimezone = "Europe/Amsterdam",
                DefaultDateTimeFormat = "yyyy-MM-dd H:mm:ss",
                DefaultDateFormat = "dd/MM/yyyy"
            });
            await dbContext.SaveChangesAsync();
            Console.WriteLine("Site settings created");
        }

        if (!dbContext.Roles.Any())
        {
            await dbContext.Roles.AddAsync(new IdentityRole<int>(Roles.Admin)
            {
                NormalizedName = Roles.Admin
            });
            await dbContext.SaveChangesAsync();
            Console.WriteLine("Roles created");
        }

        if (!dbContext.Users.Any())
        {
            await appServices.AuthService().RegisterAsync(new RegisterInput
            {
                FullName = "Ozgur GUL",
                Email = "admin@admin.com",
                Password = "Asd32!#"
            });
            await appServices.AuthService().RegisterAsync(new RegisterInput
            {
                FullName = "Sergey Brin",
                Email = "sergey@brin.com",
                Password = "Asd32!#"
            });
            await appServices.AuthService().RegisterAsync(new RegisterInput
            {
                FullName = "Larry Page",
                Email = "larry@page.com",
                Password = "Asd32!#"
            });
            await appServices.AuthService().RegisterAsync(new RegisterInput
            {
                FullName = "Mark Zuckerberg",
                Email = "mark@zuckerberg.com",
                Password = "Asd32!#"
            });
            await appServices.UserService().AddUserToRoles(new AddUserToRoleInput()
            {
                Roles = new[] { Roles.Admin },
                UserId = 1
            });
            await dbContext.SaveChangesAsync();
        }

        if (!dbContext.Projects.Any())
        {
            await appServices.ProjectService().CreateProject(1, new CreateProjectInput
            {
                Name = "My Project",
                Slug = "PR"
            });
            await appServices.BoardService().CreateBoard(1, new CreateBoardInput
            {
                Name = "Team A Board",
                ProjectId = 1
            });
            await appServices.BoardService().CreateBoard(1, new CreateBoardInput
            {
                Name = "Team B",
                ProjectId = 1
            });
            await appServices.BoardService().CreateBoard(1, new CreateBoardInput
            {
                Name = "Various board",
                ProjectId = 1
            });
            await appServices.ColumnService().CreateColumn(1, new CreateColumnInput
            {
                Name = "TODO",
                ProjectId = 1,
                BoardId = 1
            });
            await appServices.ColumnService().CreateColumn(1, new CreateColumnInput
            {
                Name = "In Progress",
                ProjectId = 1,
                BoardId = 1
            });
            await appServices.ColumnService().CreateColumn(1, new CreateColumnInput
            {
                Name = "Done",
                ProjectId = 1,
                BoardId = 1
            });
            await appServices.ColumnService().CreateColumn(1, new CreateColumnInput
            {
                Name = "Random",
                ProjectId = 1,
                BoardId = 2
            });
            // await appServices.TaskService().CreateTask(1, new CreateTaskInput
            // {
            //     ProjectId = 1,
            //     BoardId = 1,
            //     ColumnId = 1,
            //     Title = "First task, Yay!",
            //     Description = "Some description"
            // });
            // await appServices.TaskService().CreateTask(1, new CreateTaskInput
            // {
            //     ProjectId = 1,
            //     BoardId = 1,
            //     ColumnId = 1,
            //     Title = "Second task, Yay!",
            //     Description = "Some description"
            // });
            // await appServices.TaskService().CreateTask(1, new CreateTaskInput
            // {
            //     ProjectId = 1,
            //     BoardId = 2,
            //     ColumnId = 4,
            //     Title = "Third task, Yay!",
            //     Description = "Some description"
            // });
            await appServices.ProjectService().CreateProjectAssignee(new CreateProjectAssigneeInput
            {
                Ids = new List<CreateProjectAssigneeInput.UserIdDto>(
                    new List<CreateProjectAssigneeInput.UserIdDto>()
                    {
                        new CreateProjectAssigneeInput.UserIdDto()
                        {
                            UserId = 2
                        }
                    }),
                ProjectId = 1
            });
            Console.WriteLine("Projects created");

            await appServices.TaskRelationTypeDefService().CreateTaskRelationTypeDef(
                new CreateTaskRelationTypeDefInput
                {
                    Type = "Blocks",
                    InwardOperationName = "Blocks",
                    OutwardOperationName = "Blocked by"
                });
            await appServices.TaskRelationTypeDefService().CreateTaskRelationTypeDef(
                new CreateTaskRelationTypeDefInput
                {
                    Type = "Blocked by",
                    InwardOperationName = "Blocked by",
                    OutwardOperationName = "Blocks"
                });
            Console.WriteLine("Task relation type defs created");
            await dbContext.SaveChangesAsync();
        }
    }
}