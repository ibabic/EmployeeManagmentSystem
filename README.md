# Employee Management System App

## Description

This project aims to develop an Employee Management System application. The app facilitates various administrative tasks such as adding new employees, managing departments, branches, countries, and cities. Additionally, it allows users to handle employee-related functionalities like tracking vacation days, sick leave, and overtime.

### User Roles

The application is designed with three distinct roles:

- Admin: Admin users have privileged access to the system, including the ability to manage user roles.
- User: The primary role responsible for adding new employees, managing departments, branches, countries, cities, and handling employee-related tasks.
- Employee: Standard employees who can view their schedules, request time off, and overtime.

### Future Plans

#### Enhanced Calendar Functionality

- Notifications: Employees will receive notifications regarding scheduled vacations and overtime assignments.
- Vacation and Overtime Proposals: Employees can propose vacation time and overtime directly through the app.
- Employee Schedule View: Employees will have access to a calendar displaying all scheduled activities, including vacations, sick leave, overtime, and meetings for any selected date. This feature provides employees with a comprehensive overview of their daily schedule, allowing them to effectively manage their time and commitments.

Scheduled Activities:
- Vacations: Displays dates when employees are on vacation.
- Sick Leave: Indicates days when employees are on sick leave.
- Overtime: Shows overtime hours scheduled for specific dates.
- Meetings: Lists proposed and approved meeting schedules.

#### Team Management Features

- Head of Team Notifications: Team leaders or department heads will receive notifications when their team members request vacation or overtime.
- Approval Workflow: Team leaders can approve or reject vacation and overtime requests submitted by their team members.

#### Meeting Scheduling

- Meeting Proposal: Employees can propose meetings for specific dates and times through the calendar feature.
- Approval Process: Meeting proposals require approval from authorized users within the application.

## Technologies Used

The application utilizes the following technologies:

- Entity Framework (Code-First Approach) 
- ASP.NET Web API
- JWT
- Blazor Web App (Syncfusion) 
- .NET Core 8.0
- C#

## Setting Up the Application

- Configure database connection: Set up the database connection string in the appsettings.json file located in the Server project.
- Run Entity Framework migrations: Execute the necessary migrations to create or update the database schema using Entity Framework Code-First approach.
- Configure JWT token: Adjust JWT token configuration in the appsettings.json file within the Server project.
- Configure Syncfusion: Adjust Syncfusion configuration in the appsettings.json file located in the Client project.
- Start the server: Run the server-side project in the appropriate development environment (e.g., Visual Studio).
- Start the client: Launch the client-side project to initiate the frontend component of the application.

## Contribution

Contributions to this project are welcome! If you have any suggestions, find bugs, or want to contribute to the development, feel free to submit a pull request or open an issue on GitHub.
