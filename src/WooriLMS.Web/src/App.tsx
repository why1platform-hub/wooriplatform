import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import { AuthProvider, useAuth } from './context/AuthContext';
import Layout from './components/Layout';
import ProtectedRoute from './components/ProtectedRoute';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import Courses from './pages/Courses';
import CourseDetail from './pages/CourseDetail';
import Consultants from './pages/Consultants';
import Discussions from './pages/Discussions';
import Profile from './pages/Profile';

function PublicRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div className="flex h-screen items-center justify-center">
        <div className="h-12 w-12 animate-spin rounded-full border-4 border-primary-200 border-t-primary-600"></div>
      </div>
    );
  }

  if (isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
}

function AppRoutes() {
  return (
    <Routes>
      <Route
        path="/login"
        element={
          <PublicRoute>
            <Login />
          </PublicRoute>
        }
      />
      <Route
        path="/register"
        element={
          <PublicRoute>
            <Register />
          </PublicRoute>
        }
      />
      <Route
        path="/"
        element={
          <ProtectedRoute>
            <Layout />
          </ProtectedRoute>
        }
      >
        <Route index element={<Dashboard />} />
        <Route path="courses" element={<Courses />} />
        <Route path="courses/:id" element={<CourseDetail />} />
        <Route path="programs" element={<ProgramsPlaceholder />} />
        <Route path="programs/:id" element={<ProgramDetailPlaceholder />} />
        <Route path="jobs" element={<JobsPlaceholder />} />
        <Route path="jobs/:id" element={<JobDetailPlaceholder />} />
        <Route path="consultants" element={<Consultants />} />
        <Route path="discussions" element={<Discussions />} />
        <Route path="discussions/:id" element={<DiscussionDetailPlaceholder />} />
        <Route path="faq" element={<FAQPlaceholder />} />
        <Route path="announcements" element={<AnnouncementsPlaceholder />} />
        <Route path="profile" element={<Profile />} />
        <Route path="settings" element={<SettingsPlaceholder />} />

        {/* Instructor Routes */}
        <Route
          path="instructor/courses"
          element={
            <ProtectedRoute roles={['Instructor', 'Admin']}>
              <InstructorCoursesPlaceholder />
            </ProtectedRoute>
          }
        />
        <Route
          path="instructor/consultations"
          element={
            <ProtectedRoute roles={['Instructor', 'Admin']}>
              <InstructorConsultationsPlaceholder />
            </ProtectedRoute>
          }
        />

        {/* Admin Routes */}
        <Route
          path="admin/users"
          element={
            <ProtectedRoute roles={['Admin']}>
              <AdminUsersPlaceholder />
            </ProtectedRoute>
          }
        />
        <Route
          path="admin/courses"
          element={
            <ProtectedRoute roles={['Admin']}>
              <AdminCoursesPlaceholder />
            </ProtectedRoute>
          }
        />
        <Route
          path="admin/programs"
          element={
            <ProtectedRoute roles={['Admin']}>
              <AdminProgramsPlaceholder />
            </ProtectedRoute>
          }
        />
        <Route
          path="admin/jobs"
          element={
            <ProtectedRoute roles={['Admin']}>
              <AdminJobsPlaceholder />
            </ProtectedRoute>
          }
        />
        <Route
          path="admin/announcements"
          element={
            <ProtectedRoute roles={['Admin']}>
              <AdminAnnouncementsPlaceholder />
            </ProtectedRoute>
          }
        />
        <Route
          path="admin/faq"
          element={
            <ProtectedRoute roles={['Admin']}>
              <AdminFAQPlaceholder />
            </ProtectedRoute>
          }
        />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}

// Placeholder components for pages to be implemented
function ProgramsPlaceholder() {
  return <div className="text-center py-12"><h1 className="text-2xl font-bold">Programs</h1><p className="text-gray-500 mt-2">Coming soon...</p></div>;
}

function ProgramDetailPlaceholder() {
  return <div className="text-center py-12"><h1 className="text-2xl font-bold">Program Details</h1><p className="text-gray-500 mt-2">Coming soon...</p></div>;
}

function JobsPlaceholder() {
  return <div className="text-center py-12"><h1 className="text-2xl font-bold">Jobs</h1><p className="text-gray-500 mt-2">Coming soon...</p></div>;
}

function JobDetailPlaceholder() {
  return <div className="text-center py-12"><h1 className="text-2xl font-bold">Job Details</h1><p className="text-gray-500 mt-2">Coming soon...</p></div>;
}

function DiscussionDetailPlaceholder() {
  return <div className="text-center py-12"><h1 className="text-2xl font-bold">Discussion Details</h1><p className="text-gray-500 mt-2">Coming soon...</p></div>;
}

function FAQPlaceholder() {
  return <div className="text-center py-12"><h1 className="text-2xl font-bold">FAQ</h1><p className="text-gray-500 mt-2">Coming soon...</p></div>;
}

function AnnouncementsPlaceholder() {
  return <div className="text-center py-12"><h1 className="text-2xl font-bold">Announcements</h1><p className="text-gray-500 mt-2">Coming soon...</p></div>;
}

function SettingsPlaceholder() {
  return <div className="text-center py-12"><h1 className="text-2xl font-bold">Settings</h1><p className="text-gray-500 mt-2">Coming soon...</p></div>;
}

function InstructorCoursesPlaceholder() {
  return <div className="text-center py-12"><h1 className="text-2xl font-bold">My Courses (Instructor)</h1><p className="text-gray-500 mt-2">Coming soon...</p></div>;
}

function InstructorConsultationsPlaceholder() {
  return <div className="text-center py-12"><h1 className="text-2xl font-bold">Consultations (Instructor)</h1><p className="text-gray-500 mt-2">Coming soon...</p></div>;
}

function AdminUsersPlaceholder() {
  return <div className="text-center py-12"><h1 className="text-2xl font-bold">Manage Users</h1><p className="text-gray-500 mt-2">Coming soon...</p></div>;
}

function AdminCoursesPlaceholder() {
  return <div className="text-center py-12"><h1 className="text-2xl font-bold">Manage Courses</h1><p className="text-gray-500 mt-2">Coming soon...</p></div>;
}

function AdminProgramsPlaceholder() {
  return <div className="text-center py-12"><h1 className="text-2xl font-bold">Manage Programs</h1><p className="text-gray-500 mt-2">Coming soon...</p></div>;
}

function AdminJobsPlaceholder() {
  return <div className="text-center py-12"><h1 className="text-2xl font-bold">Manage Jobs</h1><p className="text-gray-500 mt-2">Coming soon...</p></div>;
}

function AdminAnnouncementsPlaceholder() {
  return <div className="text-center py-12"><h1 className="text-2xl font-bold">Manage Announcements</h1><p className="text-gray-500 mt-2">Coming soon...</p></div>;
}

function AdminFAQPlaceholder() {
  return <div className="text-center py-12"><h1 className="text-2xl font-bold">Manage FAQ</h1><p className="text-gray-500 mt-2">Coming soon...</p></div>;
}

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AppRoutes />
        <Toaster position="top-right" />
      </AuthProvider>
    </BrowserRouter>
  );
}
