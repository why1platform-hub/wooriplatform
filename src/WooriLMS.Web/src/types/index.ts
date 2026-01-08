export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  profileImageUrl?: string;
  bio?: string;
  skills?: string;
  workExperience?: string;
  education?: string;
  linkedInUrl?: string;
  resumeUrl?: string;
  phoneNumber?: string;
  dateOfBirth: string;
  userType: string;
  role: string;
  isActive: boolean;
  createdAt: string;
}

export interface AuthResponse {
  success: boolean;
  token?: string;
  expiration?: string;
  user?: User;
  errors: string[];
}

export interface Course {
  id: number;
  title: string;
  description: string;
  thumbnailUrl?: string;
  category: string;
  level: string;
  durationMinutes: number;
  isPublished: boolean;
  createdAt: string;
  instructorId: string;
  instructorName: string;
  enrollmentCount: number;
  moduleCount: number;
  lessonCount: number;
  modules: CourseModule[];
}

export interface CourseModule {
  id: number;
  courseId: number;
  title: string;
  description?: string;
  orderIndex: number;
  lessons: Lesson[];
}

export interface Lesson {
  id: number;
  moduleId: number;
  title: string;
  description?: string;
  videoUrl: string;
  durationMinutes: number;
  orderIndex: number;
  isCompleted?: boolean;
  watchedSeconds?: number;
}

export interface Enrollment {
  id: number;
  userId: string;
  userName: string;
  courseId: number;
  courseTitle: string;
  enrolledAt: string;
  completedAt?: string;
  progressPercentage: number;
  status: string;
}

export interface Consultant {
  id: string;
  name: string;
  profileImageUrl?: string;
  bio?: string;
  skills?: string;
  availableSlotsCount: number;
}

export interface TimeSlot {
  id: number;
  instructorId: string;
  instructorName: string;
  startTime: string;
  endTime: string;
  isAvailable: boolean;
  notes?: string;
  booking?: Booking;
}

export interface Booking {
  id: number;
  timeSlotId: number;
  userId: string;
  userName: string;
  userEmail?: string;
  instructorId: string;
  instructorName: string;
  startTime: string;
  endTime: string;
  topic: string;
  description?: string;
  status: string;
  meetingUrl?: string;
  cancellationReason?: string;
  createdAt: string;
  approvedAt?: string;
}

export interface Discussion {
  id: number;
  userId: string;
  userName: string;
  userProfileImage?: string;
  title: string;
  content: string;
  category: string;
  viewCount: number;
  replyCount: number;
  isPinned: boolean;
  isClosed: boolean;
  createdAt: string;
  updatedAt?: string;
  replies: DiscussionReply[];
}

export interface DiscussionReply {
  id: number;
  discussionId: number;
  userId: string;
  userName: string;
  userProfileImage?: string;
  userRole: string;
  content: string;
  isAcceptedAnswer: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface FAQ {
  id: number;
  question: string;
  answer: string;
  category: string;
  orderIndex: number;
  isPublished: boolean;
  createdAt: string;
}

export interface SkillProgram {
  id: number;
  title: string;
  description: string;
  imageUrl?: string;
  industry: string;
  durationWeeks: number;
  startDate: string;
  endDate: string;
  maxParticipants: number;
  currentParticipants: number;
  isActive: boolean;
  createdAt: string;
  courses: ProgramCourse[];
}

export interface ProgramCourse {
  id: number;
  courseId: number;
  courseTitle: string;
  orderIndex: number;
}

export interface ProgramApplication {
  id: number;
  programId: number;
  programTitle: string;
  userId: string;
  userName: string;
  userEmail?: string;
  coverLetter?: string;
  status: string;
  reviewNotes?: string;
  appliedAt: string;
  reviewedAt?: string;
}

export interface Job {
  id: number;
  title: string;
  company: string;
  description: string;
  location: string;
  salaryRange?: string;
  jobType: string;
  requiredSkills?: string;
  isActive: boolean;
  postedAt: string;
  expiresAt?: string;
  applicationCount: number;
}

export interface JobApplication {
  id: number;
  jobId: number;
  jobTitle: string;
  company: string;
  userId: string;
  userName: string;
  userEmail?: string;
  coverLetter?: string;
  resumeUrl?: string;
  status: string;
  reviewNotes?: string;
  appliedAt: string;
  reviewedAt?: string;
}

export interface Announcement {
  id: number;
  title: string;
  content: string;
  type: string;
  priority: string;
  isPublished: boolean;
  publishDate?: string;
  expiryDate?: string;
  createdById: string;
  createdByName: string;
  createdAt: string;
}
