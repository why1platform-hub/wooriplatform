import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || '/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Auth
export const authApi = {
  register: (data: { email: string; password: string; firstName: string; lastName: string; dateOfBirth: string }) =>
    api.post('/auth/register', data),
  login: (data: { email: string; password: string }) => api.post('/auth/login', data),
  getMe: () => api.get('/auth/me'),
  updateProfile: (data: Record<string, unknown>) => api.put('/auth/profile', data),
  changePassword: (data: { currentPassword: string; newPassword: string }) =>
    api.post('/auth/change-password', data),
  resetPassword: (data: { userId: string; newPassword: string }) => api.post('/auth/reset-password', data),
};

// Courses
export const coursesApi = {
  getAll: (includeUnpublished = false) => api.get(`/courses?includeUnpublished=${includeUnpublished}`),
  getById: (id: number) => api.get(`/courses/${id}`),
  getInstructorCourses: () => api.get('/courses/instructor'),
  create: (data: Record<string, unknown>) => api.post('/courses', data),
  update: (id: number, data: Record<string, unknown>) => api.put(`/courses/${id}`, data),
  delete: (id: number) => api.delete(`/courses/${id}`),
  createModule: (courseId: number, data: Record<string, unknown>) => api.post(`/courses/${courseId}/modules`, data),
  updateModule: (moduleId: number, data: Record<string, unknown>) => api.put(`/courses/modules/${moduleId}`, data),
  deleteModule: (moduleId: number) => api.delete(`/courses/modules/${moduleId}`),
  createLesson: (moduleId: number, data: Record<string, unknown>) => api.post(`/courses/modules/${moduleId}/lessons`, data),
  updateLesson: (lessonId: number, data: Record<string, unknown>) => api.put(`/courses/lessons/${lessonId}`, data),
  deleteLesson: (lessonId: number) => api.delete(`/courses/lessons/${lessonId}`),
  enroll: (courseId: number) => api.post(`/courses/${courseId}/enroll`),
  getEnrollments: () => api.get('/courses/enrollments'),
  getCourseEnrollments: (courseId: number) => api.get(`/courses/${courseId}/enrollments`),
  updateProgress: (data: { lessonId: number; watchedSeconds: number; isCompleted: boolean }) =>
    api.post('/courses/progress', data),
  unenroll: (courseId: number) => api.delete(`/courses/${courseId}/unenroll`),
};

// Consultants
export const consultantsApi = {
  getAll: () => api.get('/consultants'),
  getAvailableTimeSlots: (instructorId?: string) =>
    api.get(`/consultants/timeslots${instructorId ? `?instructorId=${instructorId}` : ''}`),
  getMyTimeSlots: () => api.get('/consultants/my-timeslots'),
  createTimeSlot: (data: Record<string, unknown>) => api.post('/consultants/timeslots', data),
  createMultipleTimeSlots: (data: Record<string, unknown>) => api.post('/consultants/timeslots/bulk', data),
  deleteTimeSlot: (id: number) => api.delete(`/consultants/timeslots/${id}`),
  createBooking: (data: { timeSlotId: number; topic: string; description?: string }) =>
    api.post('/consultants/bookings', data),
  getMyBookings: () => api.get('/consultants/bookings'),
  getInstructorBookings: () => api.get('/consultants/bookings/instructor'),
  updateBookingStatus: (id: number, data: { status: string; meetingUrl?: string; cancellationReason?: string }) =>
    api.put(`/consultants/bookings/${id}/status`, data),
  cancelBooking: (id: number, reason?: string) => api.post(`/consultants/bookings/${id}/cancel`, reason),
};

// Discussions
export const discussionsApi = {
  getAll: (category?: string) => api.get(`/discussions${category ? `?category=${category}` : ''}`),
  getById: (id: number) => api.get(`/discussions/${id}`),
  create: (data: { title: string; content: string; category?: string }) => api.post('/discussions', data),
  update: (id: number, data: Record<string, unknown>) => api.put(`/discussions/${id}`, data),
  delete: (id: number) => api.delete(`/discussions/${id}`),
  createReply: (discussionId: number, data: { content: string }) =>
    api.post(`/discussions/${discussionId}/replies`, data),
  deleteReply: (replyId: number) => api.delete(`/discussions/replies/${replyId}`),
  markAsAccepted: (replyId: number) => api.post(`/discussions/replies/${replyId}/accept`),
};

// FAQs
export const faqsApi = {
  getAll: (includeUnpublished = false) => api.get(`/faqs?includeUnpublished=${includeUnpublished}`),
  getById: (id: number) => api.get(`/faqs/${id}`),
  getByCategory: (category: string) => api.get(`/faqs/category/${category}`),
  create: (data: Record<string, unknown>) => api.post('/faqs', data),
  update: (id: number, data: Record<string, unknown>) => api.put(`/faqs/${id}`, data),
  delete: (id: number) => api.delete(`/faqs/${id}`),
};

// Programs
export const programsApi = {
  getAll: (includeInactive = false) => api.get(`/programs?includeInactive=${includeInactive}`),
  getById: (id: number) => api.get(`/programs/${id}`),
  create: (data: Record<string, unknown>) => api.post('/programs', data),
  update: (id: number, data: Record<string, unknown>) => api.put(`/programs/${id}`, data),
  delete: (id: number) => api.delete(`/programs/${id}`),
  addCourse: (programId: number, courseId: number, orderIndex = 0) =>
    api.post(`/programs/${programId}/courses/${courseId}?orderIndex=${orderIndex}`),
  removeCourse: (programId: number, courseId: number) =>
    api.delete(`/programs/${programId}/courses/${courseId}`),
  apply: (programId: number, data: { coverLetter?: string }) =>
    api.post(`/programs/${programId}/apply`, data),
  getMyApplications: () => api.get('/programs/my-applications'),
  getProgramApplications: (programId: number) => api.get(`/programs/${programId}/applications`),
  updateApplicationStatus: (applicationId: number, data: { status: string; reviewNotes?: string }) =>
    api.put(`/programs/applications/${applicationId}/status`, data),
};

// Jobs
export const jobsApi = {
  getAll: (includeInactive = false) => api.get(`/jobs?includeInactive=${includeInactive}`),
  getById: (id: number) => api.get(`/jobs/${id}`),
  create: (data: Record<string, unknown>) => api.post('/jobs', data),
  update: (id: number, data: Record<string, unknown>) => api.put(`/jobs/${id}`, data),
  delete: (id: number) => api.delete(`/jobs/${id}`),
  apply: (jobId: number, data: { coverLetter?: string; resumeUrl?: string }) =>
    api.post(`/jobs/${jobId}/apply`, data),
  getMyApplications: () => api.get('/jobs/my-applications'),
  getJobApplications: (jobId: number) => api.get(`/jobs/${jobId}/applications`),
  updateApplicationStatus: (applicationId: number, data: { status: string; reviewNotes?: string }) =>
    api.put(`/jobs/applications/${applicationId}/status`, data),
};

// Announcements
export const announcementsApi = {
  getAll: (includeUnpublished = false) => api.get(`/announcements?includeUnpublished=${includeUnpublished}`),
  getById: (id: number) => api.get(`/announcements/${id}`),
  create: (data: Record<string, unknown>) => api.post('/announcements', data),
  update: (id: number, data: Record<string, unknown>) => api.put(`/announcements/${id}`, data),
  delete: (id: number) => api.delete(`/announcements/${id}`),
};

// Users (Admin)
export const usersApi = {
  getAll: () => api.get('/users'),
  getById: (id: string) => api.get(`/users/${id}`),
  getByRole: (role: string) => api.get(`/users/role/${role}`),
  update: (id: string, data: Record<string, unknown>) => api.put(`/users/${id}`, data),
  updateRole: (id: string, role: string) => api.put(`/users/${id}/role`, { role }),
  toggleStatus: (id: string) => api.put(`/users/${id}/toggle-status`),
  resetPassword: (id: string, newPassword: string) =>
    api.post(`/users/${id}/reset-password`, { newPassword }),
  delete: (id: string) => api.delete(`/users/${id}`),
};

export default api;
