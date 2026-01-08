import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import {
  BookOpenIcon,
  AcademicCapIcon,
  BriefcaseIcon,
  UserGroupIcon,
  CalendarIcon,
  ChartBarIcon,
} from '@heroicons/react/24/outline';
import { useAuth } from '../context/AuthContext';
import { coursesApi, announcementsApi, programsApi, jobsApi } from '../services/api';
import { Enrollment, Announcement, SkillProgram, Job } from '../types';
import { format } from 'date-fns';

export default function Dashboard() {
  const { user } = useAuth();
  const [enrollments, setEnrollments] = useState<Enrollment[]>([]);
  const [announcements, setAnnouncements] = useState<Announcement[]>([]);
  const [programs, setPrograms] = useState<SkillProgram[]>([]);
  const [jobs, setJobs] = useState<Job[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [enrollmentsRes, announcementsRes, programsRes, jobsRes] = await Promise.all([
          coursesApi.getEnrollments(),
          announcementsApi.getAll(),
          programsApi.getAll(),
          jobsApi.getAll(),
        ]);
        setEnrollments(enrollmentsRes.data);
        setAnnouncements(announcementsRes.data.slice(0, 3));
        setPrograms(programsRes.data.slice(0, 3));
        setJobs(jobsRes.data.slice(0, 3));
      } catch (error) {
        console.error('Error fetching dashboard data:', error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, []);

  const stats = [
    { name: 'Enrolled Courses', value: enrollments.length, icon: BookOpenIcon, href: '/courses' },
    { name: 'Completed', value: enrollments.filter(e => e.status === 'Completed').length, icon: ChartBarIcon, href: '/courses' },
    { name: 'In Progress', value: enrollments.filter(e => e.status === 'Active').length, icon: CalendarIcon, href: '/courses' },
  ];

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <div className="h-12 w-12 animate-spin rounded-full border-4 border-primary-200 border-t-primary-600"></div>
      </div>
    );
  }

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-gray-900">
          Welcome back, {user?.firstName}!
        </h1>
        <p className="mt-1 text-sm text-gray-500">
          Continue your learning journey and explore new opportunities.
        </p>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-3 mb-8">
        {stats.map((stat) => (
          <Link
            key={stat.name}
            to={stat.href}
            className="overflow-hidden rounded-lg bg-white px-4 py-5 shadow hover:shadow-md transition-shadow"
          >
            <div className="flex items-center">
              <div className="flex-shrink-0">
                <stat.icon className="h-8 w-8 text-primary-600" aria-hidden="true" />
              </div>
              <div className="ml-5 w-0 flex-1">
                <dt className="truncate text-sm font-medium text-gray-500">{stat.name}</dt>
                <dd className="mt-1 text-3xl font-semibold tracking-tight text-gray-900">
                  {stat.value}
                </dd>
              </div>
            </div>
          </Link>
        ))}
      </div>

      <div className="grid grid-cols-1 gap-8 lg:grid-cols-2">
        {/* Active Courses */}
        <div className="bg-white rounded-lg shadow p-6">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-semibold text-gray-900">My Courses</h2>
            <Link to="/courses" className="text-sm text-primary-600 hover:text-primary-500">
              View all
            </Link>
          </div>
          {enrollments.length === 0 ? (
            <p className="text-gray-500 text-sm">
              You haven't enrolled in any courses yet.{' '}
              <Link to="/courses" className="text-primary-600 hover:text-primary-500">
                Browse courses
              </Link>
            </p>
          ) : (
            <ul className="divide-y divide-gray-200">
              {enrollments.slice(0, 3).map((enrollment) => (
                <li key={enrollment.id} className="py-3">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-sm font-medium text-gray-900">{enrollment.courseTitle}</p>
                      <p className="text-xs text-gray-500">
                        Enrolled {format(new Date(enrollment.enrolledAt), 'MMM d, yyyy')}
                      </p>
                    </div>
                    <div className="flex items-center">
                      <div className="w-24 bg-gray-200 rounded-full h-2 mr-2">
                        <div
                          className="bg-primary-600 h-2 rounded-full"
                          style={{ width: `${enrollment.progressPercentage}%` }}
                        ></div>
                      </div>
                      <span className="text-xs text-gray-500">{enrollment.progressPercentage}%</span>
                    </div>
                  </div>
                </li>
              ))}
            </ul>
          )}
        </div>

        {/* Announcements */}
        <div className="bg-white rounded-lg shadow p-6">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-semibold text-gray-900">Announcements</h2>
            <Link to="/announcements" className="text-sm text-primary-600 hover:text-primary-500">
              View all
            </Link>
          </div>
          {announcements.length === 0 ? (
            <p className="text-gray-500 text-sm">No announcements at this time.</p>
          ) : (
            <ul className="divide-y divide-gray-200">
              {announcements.map((announcement) => (
                <li key={announcement.id} className="py-3">
                  <p className="text-sm font-medium text-gray-900">{announcement.title}</p>
                  <p className="text-xs text-gray-500 mt-1 line-clamp-2">{announcement.content}</p>
                  <p className="text-xs text-gray-400 mt-1">
                    {format(new Date(announcement.createdAt), 'MMM d, yyyy')}
                  </p>
                </li>
              ))}
            </ul>
          )}
        </div>

        {/* Programs */}
        <div className="bg-white rounded-lg shadow p-6">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-semibold text-gray-900 flex items-center">
              <AcademicCapIcon className="h-5 w-5 mr-2 text-primary-600" />
              Skill Programs
            </h2>
            <Link to="/programs" className="text-sm text-primary-600 hover:text-primary-500">
              View all
            </Link>
          </div>
          {programs.length === 0 ? (
            <p className="text-gray-500 text-sm">No programs available at this time.</p>
          ) : (
            <ul className="divide-y divide-gray-200">
              {programs.map((program) => (
                <li key={program.id} className="py-3">
                  <Link to={`/programs/${program.id}`} className="hover:text-primary-600">
                    <p className="text-sm font-medium text-gray-900">{program.title}</p>
                    <p className="text-xs text-gray-500 mt-1">{program.industry} · {program.durationWeeks} weeks</p>
                  </Link>
                </li>
              ))}
            </ul>
          )}
        </div>

        {/* Jobs */}
        <div className="bg-white rounded-lg shadow p-6">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-semibold text-gray-900 flex items-center">
              <BriefcaseIcon className="h-5 w-5 mr-2 text-primary-600" />
              Job Opportunities
            </h2>
            <Link to="/jobs" className="text-sm text-primary-600 hover:text-primary-500">
              View all
            </Link>
          </div>
          {jobs.length === 0 ? (
            <p className="text-gray-500 text-sm">No job listings at this time.</p>
          ) : (
            <ul className="divide-y divide-gray-200">
              {jobs.map((job) => (
                <li key={job.id} className="py-3">
                  <Link to={`/jobs/${job.id}`} className="hover:text-primary-600">
                    <p className="text-sm font-medium text-gray-900">{job.title}</p>
                    <p className="text-xs text-gray-500 mt-1">{job.company} · {job.location}</p>
                  </Link>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>

      {/* Quick Actions */}
      <div className="mt-8 bg-primary-50 rounded-lg p-6">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">Quick Actions</h2>
        <div className="grid grid-cols-2 gap-4 sm:grid-cols-4">
          <Link
            to="/courses"
            className="flex flex-col items-center p-4 bg-white rounded-lg shadow-sm hover:shadow-md transition-shadow"
          >
            <BookOpenIcon className="h-8 w-8 text-primary-600 mb-2" />
            <span className="text-sm font-medium text-gray-900">Browse Courses</span>
          </Link>
          <Link
            to="/consultants"
            className="flex flex-col items-center p-4 bg-white rounded-lg shadow-sm hover:shadow-md transition-shadow"
          >
            <UserGroupIcon className="h-8 w-8 text-primary-600 mb-2" />
            <span className="text-sm font-medium text-gray-900">Book Consultant</span>
          </Link>
          <Link
            to="/discussions"
            className="flex flex-col items-center p-4 bg-white rounded-lg shadow-sm hover:shadow-md transition-shadow"
          >
            <AcademicCapIcon className="h-8 w-8 text-primary-600 mb-2" />
            <span className="text-sm font-medium text-gray-900">Ask Question</span>
          </Link>
          <Link
            to="/profile"
            className="flex flex-col items-center p-4 bg-white rounded-lg shadow-sm hover:shadow-md transition-shadow"
          >
            <BriefcaseIcon className="h-8 w-8 text-primary-600 mb-2" />
            <span className="text-sm font-medium text-gray-900">My Profile</span>
          </Link>
        </div>
      </div>
    </div>
  );
}
