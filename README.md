# Woori LMS - Learning Management System for Retirees

A comprehensive Learning Management System designed to help retirees skill up and transition to new career opportunities.

## Features

### For Normal Users
- **Video Learning (LMS)**: Browse and enroll in courses, watch video lessons, track progress
- **Book a Consultant**: Schedule one-on-one sessions with career consultants
- **Programs**: Apply for skill development programs
- **Jobs**: Browse and apply for job opportunities
- **Discussion Q&A**: Ask questions and engage with the community
- **FAQ**: Access frequently asked questions
- **My Profile**: Manage personal information, skills, and resume

### For Instructors
- **Upload Classes**: Create courses with modules and video lessons
- **Manage Consultations**: Set availability and approve consultation requests

### For Administrators
- **User Management**: Manage all users, roles, and account settings (including password reset)
- **Course Management**: Oversee all courses in the system
- **Program Management**: Create and manage skill development programs
- **Job Management**: Post and manage job listings
- **Announcements**: Create and manage platform announcements
- **FAQ Management**: Manage FAQ content

## Technology Stack

### Backend
- ASP.NET Core 8.0 Web API
- Entity Framework Core with SQL Server
- ASP.NET Core Identity for authentication
- JWT Bearer token authentication
- Swagger/OpenAPI documentation

### Frontend
- React 18 with TypeScript
- Vite build tool
- Tailwind CSS for styling
- React Router for navigation
- React Hook Form for form handling
- Axios for API communication

## Project Structure

```
WooriLMS/
├── WooriLMS.sln                    # Visual Studio solution file
├── README.md
└── src/
    ├── WooriLMS.API/               # ASP.NET Core Web API
    │   ├── Controllers/            # API Controllers
    │   ├── Data/                   # DbContext and seed data
    │   ├── DTOs/                   # Data Transfer Objects
    │   ├── Models/                 # Domain entities
    │   ├── Services/               # Business logic services
    │   └── Properties/
    └── WooriLMS.Web/               # React TypeScript frontend
        ├── src/
        │   ├── components/         # Reusable components
        │   ├── context/            # React context providers
        │   ├── pages/              # Page components
        │   ├── services/           # API service layer
        │   └── types/              # TypeScript type definitions
        └── public/
```

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Node.js 18+ and npm
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Backend Setup

1. Open `WooriLMS.sln` in Visual Studio

2. Update the connection string in `appsettings.json` if needed:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WooriLMS;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

3. Run Entity Framework migrations:
```bash
cd src/WooriLMS.API
dotnet ef migrations add InitialCreate
dotnet ef database update
```

4. Run the API:
```bash
dotnet run
```

The API will be available at `https://localhost:7001` with Swagger UI at `https://localhost:7001/swagger`

### Frontend Setup

1. Navigate to the frontend project:
```bash
cd src/WooriLMS.Web
```

2. Install dependencies:
```bash
npm install
```

3. Start the development server:
```bash
npm run dev
```

The frontend will be available at `http://localhost:3000`

## Default Users

After running the application, the following users are seeded:

| Email | Password | Role |
|-------|----------|------|
| admin@woorilms.com | Admin@123 | Admin |
| instructor@woorilms.com | Instructor@123 | Instructor |

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login
- `GET /api/auth/me` - Get current user
- `PUT /api/auth/profile` - Update profile
- `POST /api/auth/change-password` - Change password
- `POST /api/auth/reset-password` - Reset password (Admin only)

### Courses
- `GET /api/courses` - Get all courses
- `GET /api/courses/{id}` - Get course by ID
- `POST /api/courses` - Create course (Instructor/Admin)
- `PUT /api/courses/{id}` - Update course
- `DELETE /api/courses/{id}` - Delete course (Admin)
- `POST /api/courses/{id}/enroll` - Enroll in course
- `POST /api/courses/progress` - Update lesson progress

### Consultants
- `GET /api/consultants` - Get all consultants
- `GET /api/consultants/timeslots` - Get available time slots
- `POST /api/consultants/timeslots` - Create time slot (Instructor)
- `POST /api/consultants/bookings` - Create booking
- `PUT /api/consultants/bookings/{id}/status` - Update booking status (Instructor)

### Discussions
- `GET /api/discussions` - Get all discussions
- `GET /api/discussions/{id}` - Get discussion by ID
- `POST /api/discussions` - Create discussion
- `POST /api/discussions/{id}/replies` - Add reply

### Programs
- `GET /api/programs` - Get all programs
- `POST /api/programs/{id}/apply` - Apply to program
- `PUT /api/programs/applications/{id}/status` - Update application status (Admin)

### Jobs
- `GET /api/jobs` - Get all jobs
- `POST /api/jobs/{id}/apply` - Apply to job
- `PUT /api/jobs/applications/{id}/status` - Update application status (Admin)

### Admin
- `GET /api/users` - Get all users
- `PUT /api/users/{id}/role` - Update user role
- `PUT /api/users/{id}/toggle-status` - Toggle user active status
- `POST /api/users/{id}/reset-password` - Reset user password

## License

This project is proprietary software developed for Woori Platform.
