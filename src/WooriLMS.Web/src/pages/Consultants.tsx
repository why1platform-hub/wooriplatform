import { useEffect, useState } from 'react';
import { format } from 'date-fns';
import { CalendarIcon, ClockIcon, UserIcon } from '@heroicons/react/24/outline';
import toast from 'react-hot-toast';
import { consultantsApi } from '../services/api';
import { Consultant, TimeSlot, Booking } from '../types';

export default function Consultants() {
  const [consultants, setConsultants] = useState<Consultant[]>([]);
  const [timeSlots, setTimeSlots] = useState<TimeSlot[]>([]);
  const [myBookings, setMyBookings] = useState<Booking[]>([]);
  const [selectedConsultant, setSelectedConsultant] = useState<string>('');
  const [selectedSlot, setSelectedSlot] = useState<TimeSlot | null>(null);
  const [bookingForm, setBookingForm] = useState({ topic: '', description: '' });
  const [isLoading, setIsLoading] = useState(true);
  const [isBooking, setIsBooking] = useState(false);
  const [activeTab, setActiveTab] = useState<'book' | 'mybookings'>('book');

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [consultantsRes, bookingsRes] = await Promise.all([
          consultantsApi.getAll(),
          consultantsApi.getMyBookings(),
        ]);
        setConsultants(consultantsRes.data);
        setMyBookings(bookingsRes.data);
      } catch (error) {
        console.error('Error fetching data:', error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, []);

  useEffect(() => {
    const fetchTimeSlots = async () => {
      try {
        const response = await consultantsApi.getAvailableTimeSlots(
          selectedConsultant || undefined
        );
        setTimeSlots(response.data);
      } catch (error) {
        console.error('Error fetching time slots:', error);
      }
    };

    fetchTimeSlots();
  }, [selectedConsultant]);

  const handleBooking = async () => {
    if (!selectedSlot || !bookingForm.topic) {
      toast.error('Please select a time slot and enter a topic');
      return;
    }

    setIsBooking(true);
    try {
      await consultantsApi.createBooking({
        timeSlotId: selectedSlot.id,
        topic: bookingForm.topic,
        description: bookingForm.description,
      });
      toast.success('Booking request submitted!');
      setSelectedSlot(null);
      setBookingForm({ topic: '', description: '' });

      // Refresh data
      const [slotsRes, bookingsRes] = await Promise.all([
        consultantsApi.getAvailableTimeSlots(selectedConsultant || undefined),
        consultantsApi.getMyBookings(),
      ]);
      setTimeSlots(slotsRes.data);
      setMyBookings(bookingsRes.data);
    } catch (error: unknown) {
      const err = error as { response?: { data?: { message?: string } } };
      toast.error(err.response?.data?.message || 'Failed to book');
    } finally {
      setIsBooking(false);
    }
  };

  const handleCancelBooking = async (bookingId: number) => {
    try {
      await consultantsApi.cancelBooking(bookingId);
      toast.success('Booking cancelled');
      const bookingsRes = await consultantsApi.getMyBookings();
      setMyBookings(bookingsRes.data);
    } catch {
      toast.error('Failed to cancel booking');
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Approved':
        return 'bg-green-100 text-green-800';
      case 'Pending':
        return 'bg-yellow-100 text-yellow-800';
      case 'Rejected':
      case 'Cancelled':
        return 'bg-red-100 text-red-800';
      case 'Completed':
        return 'bg-blue-100 text-blue-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

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
        <h1 className="text-2xl font-bold text-gray-900">Book a Consultant</h1>
        <p className="mt-1 text-sm text-gray-500">
          Schedule a one-on-one session with our career consultants.
        </p>
      </div>

      {/* Tabs */}
      <div className="border-b border-gray-200 mb-6">
        <nav className="-mb-px flex space-x-8">
          <button
            onClick={() => setActiveTab('book')}
            className={`py-2 px-1 border-b-2 font-medium text-sm ${
              activeTab === 'book'
                ? 'border-primary-500 text-primary-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            }`}
          >
            Book Session
          </button>
          <button
            onClick={() => setActiveTab('mybookings')}
            className={`py-2 px-1 border-b-2 font-medium text-sm ${
              activeTab === 'mybookings'
                ? 'border-primary-500 text-primary-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            }`}
          >
            My Bookings ({myBookings.length})
          </button>
        </nav>
      </div>

      {activeTab === 'book' ? (
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Consultants List */}
          <div className="lg:col-span-1">
            <h2 className="text-lg font-semibold text-gray-900 mb-4">Available Consultants</h2>
            <div className="space-y-4">
              <button
                onClick={() => setSelectedConsultant('')}
                className={`w-full text-left p-4 rounded-lg border ${
                  selectedConsultant === ''
                    ? 'border-primary-500 bg-primary-50'
                    : 'border-gray-200 hover:border-gray-300'
                }`}
              >
                <span className="font-medium">All Consultants</span>
              </button>
              {consultants.map((consultant) => (
                <button
                  key={consultant.id}
                  onClick={() => setSelectedConsultant(consultant.id)}
                  className={`w-full text-left p-4 rounded-lg border ${
                    selectedConsultant === consultant.id
                      ? 'border-primary-500 bg-primary-50'
                      : 'border-gray-200 hover:border-gray-300'
                  }`}
                >
                  <div className="flex items-center">
                    <div className="flex-shrink-0">
                      {consultant.profileImageUrl ? (
                        <img
                          src={consultant.profileImageUrl}
                          alt={consultant.name}
                          className="h-10 w-10 rounded-full"
                        />
                      ) : (
                        <UserIcon className="h-10 w-10 text-gray-400 bg-gray-100 rounded-full p-2" />
                      )}
                    </div>
                    <div className="ml-3">
                      <p className="font-medium text-gray-900">{consultant.name}</p>
                      <p className="text-sm text-gray-500">
                        {consultant.availableSlotsCount} slots available
                      </p>
                    </div>
                  </div>
                  {consultant.bio && (
                    <p className="mt-2 text-sm text-gray-600 line-clamp-2">{consultant.bio}</p>
                  )}
                </button>
              ))}
            </div>
          </div>

          {/* Time Slots */}
          <div className="lg:col-span-2">
            <h2 className="text-lg font-semibold text-gray-900 mb-4">Available Time Slots</h2>
            {timeSlots.length === 0 ? (
              <p className="text-gray-500">No available time slots at the moment.</p>
            ) : (
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-6">
                {timeSlots.map((slot) => (
                  <button
                    key={slot.id}
                    onClick={() => setSelectedSlot(slot)}
                    className={`p-4 rounded-lg border text-left ${
                      selectedSlot?.id === slot.id
                        ? 'border-primary-500 bg-primary-50'
                        : 'border-gray-200 hover:border-gray-300'
                    }`}
                  >
                    <div className="flex items-center text-sm text-gray-600 mb-1">
                      <UserIcon className="h-4 w-4 mr-1" />
                      {slot.instructorName}
                    </div>
                    <div className="flex items-center text-sm font-medium text-gray-900">
                      <CalendarIcon className="h-4 w-4 mr-1" />
                      {format(new Date(slot.startTime), 'MMM d, yyyy')}
                    </div>
                    <div className="flex items-center text-sm text-gray-600 mt-1">
                      <ClockIcon className="h-4 w-4 mr-1" />
                      {format(new Date(slot.startTime), 'h:mm a')} -{' '}
                      {format(new Date(slot.endTime), 'h:mm a')}
                    </div>
                  </button>
                ))}
              </div>
            )}

            {/* Booking Form */}
            {selectedSlot && (
              <div className="bg-white rounded-lg shadow p-6">
                <h3 className="text-lg font-semibold text-gray-900 mb-4">Book This Slot</h3>
                <p className="text-sm text-gray-600 mb-4">
                  {selectedSlot.instructorName} -{' '}
                  {format(new Date(selectedSlot.startTime), 'MMM d, yyyy h:mm a')}
                </p>
                <div className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Topic *
                    </label>
                    <input
                      type="text"
                      value={bookingForm.topic}
                      onChange={(e) =>
                        setBookingForm({ ...bookingForm, topic: e.target.value })
                      }
                      placeholder="What would you like to discuss?"
                      className="block w-full rounded-md border-0 py-2 px-3 text-gray-900 ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-inset focus:ring-primary-600"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Description
                    </label>
                    <textarea
                      value={bookingForm.description}
                      onChange={(e) =>
                        setBookingForm({ ...bookingForm, description: e.target.value })
                      }
                      placeholder="Provide more details about your consultation needs..."
                      rows={3}
                      className="block w-full rounded-md border-0 py-2 px-3 text-gray-900 ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-inset focus:ring-primary-600"
                    />
                  </div>
                  <div className="flex gap-3">
                    <button
                      onClick={handleBooking}
                      disabled={isBooking}
                      className="flex-1 py-2 px-4 bg-primary-600 text-white rounded-md hover:bg-primary-700 disabled:opacity-50"
                    >
                      {isBooking ? 'Booking...' : 'Submit Booking Request'}
                    </button>
                    <button
                      onClick={() => setSelectedSlot(null)}
                      className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
                    >
                      Cancel
                    </button>
                  </div>
                </div>
              </div>
            )}
          </div>
        </div>
      ) : (
        /* My Bookings Tab */
        <div>
          {myBookings.length === 0 ? (
            <p className="text-gray-500">You haven't booked any consultations yet.</p>
          ) : (
            <div className="space-y-4">
              {myBookings.map((booking) => (
                <div key={booking.id} className="bg-white rounded-lg shadow p-6">
                  <div className="flex justify-between items-start">
                    <div>
                      <h3 className="font-semibold text-gray-900">{booking.topic}</h3>
                      <p className="text-sm text-gray-600 mt-1">
                        with {booking.instructorName}
                      </p>
                      <div className="flex items-center text-sm text-gray-500 mt-2">
                        <CalendarIcon className="h-4 w-4 mr-1" />
                        {format(new Date(booking.startTime), 'MMM d, yyyy')}
                        <ClockIcon className="h-4 w-4 ml-3 mr-1" />
                        {format(new Date(booking.startTime), 'h:mm a')} -{' '}
                        {format(new Date(booking.endTime), 'h:mm a')}
                      </div>
                      {booking.description && (
                        <p className="text-sm text-gray-600 mt-2">{booking.description}</p>
                      )}
                      {booking.meetingUrl && booking.status === 'Approved' && (
                        <a
                          href={booking.meetingUrl}
                          target="_blank"
                          rel="noopener noreferrer"
                          className="inline-block mt-2 text-primary-600 hover:text-primary-500 text-sm"
                        >
                          Join Meeting
                        </a>
                      )}
                    </div>
                    <div className="flex flex-col items-end gap-2">
                      <span
                        className={`px-2 py-1 text-xs font-medium rounded ${getStatusColor(
                          booking.status
                        )}`}
                      >
                        {booking.status}
                      </span>
                      {booking.status === 'Pending' && (
                        <button
                          onClick={() => handleCancelBooking(booking.id)}
                          className="text-sm text-red-600 hover:text-red-500"
                        >
                          Cancel
                        </button>
                      )}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}
    </div>
  );
}
