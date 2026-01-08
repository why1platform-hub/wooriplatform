import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import ReactPlayer from 'react-player';
import {
  ClockIcon,
  UserIcon,
  PlayIcon,
  CheckCircleIcon,
  LockClosedIcon,
} from '@heroicons/react/24/outline';
import toast from 'react-hot-toast';
import { coursesApi } from '../services/api';
import { Course, Enrollment } from '../types';
import { useAuth } from '../context/AuthContext';

export default function CourseDetail() {
  const { id } = useParams<{ id: string }>();
  useAuth();
  const [course, setCourse] = useState<Course | null>(null);
  const [enrollment, setEnrollment] = useState<Enrollment | null>(null);
  const [currentLesson, setCurrentLesson] = useState<number | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isEnrolling, setIsEnrolling] = useState(false);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const courseRes = await coursesApi.getById(Number(id));
        setCourse(courseRes.data);

        const enrollmentsRes = await coursesApi.getEnrollments();
        const userEnrollment = enrollmentsRes.data.find(
          (e: Enrollment) => e.courseId === Number(id)
        );
        setEnrollment(userEnrollment || null);

        if (userEnrollment && courseRes.data.modules[0]?.lessons[0]) {
          setCurrentLesson(courseRes.data.modules[0].lessons[0].id);
        }
      } catch (error) {
        console.error('Error fetching course:', error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, [id]);

  const handleEnroll = async () => {
    setIsEnrolling(true);
    try {
      const response = await coursesApi.enroll(Number(id));
      setEnrollment(response.data);
      toast.success('Successfully enrolled in the course!');
      if (course?.modules[0]?.lessons[0]) {
        setCurrentLesson(course.modules[0].lessons[0].id);
      }
    } catch (error: unknown) {
      const err = error as { response?: { data?: { message?: string } } };
      toast.error(err.response?.data?.message || 'Failed to enroll');
    } finally {
      setIsEnrolling(false);
    }
  };

  const handleLessonComplete = async (lessonId: number) => {
    try {
      await coursesApi.updateProgress({
        lessonId,
        watchedSeconds: 0,
        isCompleted: true,
      });
      // Refresh course data
      const courseRes = await coursesApi.getById(Number(id));
      setCourse(courseRes.data);
      const enrollmentsRes = await coursesApi.getEnrollments();
      const userEnrollment = enrollmentsRes.data.find(
        (e: Enrollment) => e.courseId === Number(id)
      );
      setEnrollment(userEnrollment || null);
      toast.success('Lesson marked as complete!');
    } catch {
      toast.error('Failed to update progress');
    }
  };

  const getCurrentLessonData = () => {
    if (!course || !currentLesson) return null;
    for (const module of course.modules) {
      const lesson = module.lessons.find((l) => l.id === currentLesson);
      if (lesson) return lesson;
    }
    return null;
  };

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <div className="h-12 w-12 animate-spin rounded-full border-4 border-primary-200 border-t-primary-600"></div>
      </div>
    );
  }

  if (!course) {
    return <div className="text-center py-12">Course not found.</div>;
  }

  const currentLessonData = getCurrentLessonData();

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-gray-900">{course.title}</h1>
        <div className="mt-2 flex flex-wrap items-center gap-4 text-sm text-gray-500">
          <span className="flex items-center">
            <UserIcon className="h-4 w-4 mr-1" />
            {course.instructorName}
          </span>
          <span className="flex items-center">
            <ClockIcon className="h-4 w-4 mr-1" />
            {Math.floor(course.durationMinutes / 60)}h {course.durationMinutes % 60}m
          </span>
          <span className="bg-primary-100 text-primary-700 px-2 py-0.5 rounded">
            {course.level}
          </span>
          <span className="bg-gray-100 px-2 py-0.5 rounded">{course.category}</span>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Main Content */}
        <div className="lg:col-span-2">
          {enrollment && currentLessonData ? (
            <div className="bg-black rounded-lg overflow-hidden mb-6">
              <ReactPlayer
                url={currentLessonData.videoUrl}
                width="100%"
                height="400px"
                controls
                onEnded={() => handleLessonComplete(currentLessonData.id)}
              />
            </div>
          ) : (
            <div className="bg-gray-100 rounded-lg h-[400px] flex items-center justify-center mb-6">
              {course.thumbnailUrl ? (
                <img
                  src={course.thumbnailUrl}
                  alt={course.title}
                  className="h-full w-full object-cover rounded-lg"
                />
              ) : (
                <div className="text-center">
                  <LockClosedIcon className="h-12 w-12 text-gray-400 mx-auto mb-2" />
                  <p className="text-gray-500">Enroll to start learning</p>
                </div>
              )}
            </div>
          )}

          {currentLessonData && (
            <div className="bg-white rounded-lg shadow p-6 mb-6">
              <h2 className="text-lg font-semibold text-gray-900">{currentLessonData.title}</h2>
              {currentLessonData.description && (
                <p className="mt-2 text-gray-600">{currentLessonData.description}</p>
              )}
              {!currentLessonData.isCompleted && enrollment && (
                <button
                  onClick={() => handleLessonComplete(currentLessonData.id)}
                  className="mt-4 inline-flex items-center px-4 py-2 border border-primary-600 text-primary-600 rounded-md hover:bg-primary-50"
                >
                  <CheckCircleIcon className="h-5 w-5 mr-2" />
                  Mark as Complete
                </button>
              )}
            </div>
          )}

          <div className="bg-white rounded-lg shadow p-6">
            <h2 className="text-lg font-semibold text-gray-900 mb-4">About this course</h2>
            <p className="text-gray-600">{course.description}</p>
          </div>
        </div>

        {/* Sidebar */}
        <div className="space-y-6">
          {/* Enrollment Card */}
          <div className="bg-white rounded-lg shadow p-6">
            {enrollment ? (
              <div>
                <div className="text-center mb-4">
                  <div className="text-3xl font-bold text-primary-600">
                    {enrollment.progressPercentage}%
                  </div>
                  <p className="text-sm text-gray-500">Complete</p>
                </div>
                <div className="w-full bg-gray-200 rounded-full h-2 mb-4">
                  <div
                    className="bg-primary-600 h-2 rounded-full"
                    style={{ width: `${enrollment.progressPercentage}%` }}
                  ></div>
                </div>
                <p className="text-sm text-gray-500 text-center">
                  {enrollment.status === 'Completed'
                    ? 'Course completed!'
                    : 'Keep going!'}
                </p>
              </div>
            ) : (
              <div>
                <p className="text-gray-600 mb-4">
                  Enroll in this course to start learning and track your progress.
                </p>
                <button
                  onClick={handleEnroll}
                  disabled={isEnrolling}
                  className="w-full py-2 px-4 bg-primary-600 text-white rounded-md hover:bg-primary-700 disabled:opacity-50"
                >
                  {isEnrolling ? 'Enrolling...' : 'Enroll Now'}
                </button>
              </div>
            )}
          </div>

          {/* Course Content */}
          <div className="bg-white rounded-lg shadow p-6">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">Course Content</h3>
            <div className="space-y-4">
              {course.modules.map((module, moduleIndex) => (
                <div key={module.id}>
                  <h4 className="font-medium text-gray-900">
                    {moduleIndex + 1}. {module.title}
                  </h4>
                  <ul className="mt-2 space-y-1">
                    {module.lessons.map((lesson) => (
                      <li key={lesson.id}>
                        <button
                          onClick={() => enrollment && setCurrentLesson(lesson.id)}
                          disabled={!enrollment}
                          className={`w-full flex items-center justify-between p-2 rounded text-sm ${
                            currentLesson === lesson.id
                              ? 'bg-primary-50 text-primary-700'
                              : 'hover:bg-gray-50'
                          } ${!enrollment ? 'opacity-50 cursor-not-allowed' : ''}`}
                        >
                          <span className="flex items-center">
                            {lesson.isCompleted ? (
                              <CheckCircleIcon className="h-4 w-4 mr-2 text-green-500" />
                            ) : enrollment ? (
                              <PlayIcon className="h-4 w-4 mr-2 text-gray-400" />
                            ) : (
                              <LockClosedIcon className="h-4 w-4 mr-2 text-gray-400" />
                            )}
                            {lesson.title}
                          </span>
                          <span className="text-xs text-gray-400">
                            {lesson.durationMinutes}m
                          </span>
                        </button>
                      </li>
                    ))}
                  </ul>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
