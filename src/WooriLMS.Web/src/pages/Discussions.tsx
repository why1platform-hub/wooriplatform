import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { format } from 'date-fns';
import {
  ChatBubbleLeftIcon,
  EyeIcon,
  PlusIcon,
  TagIcon,
} from '@heroicons/react/24/outline';
import toast from 'react-hot-toast';
import { discussionsApi } from '../services/api';
import { Discussion } from '../types';
import { useAuth } from '../context/AuthContext';

export default function Discussions() {
  useAuth();
  const [discussions, setDiscussions] = useState<Discussion[]>([]);
  const [categoryFilter, setCategoryFilter] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [showNewForm, setShowNewForm] = useState(false);
  const [newDiscussion, setNewDiscussion] = useState({
    title: '',
    content: '',
    category: 'General',
  });
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    fetchDiscussions();
  }, [categoryFilter]);

  const fetchDiscussions = async () => {
    try {
      const response = await discussionsApi.getAll(categoryFilter || undefined);
      setDiscussions(response.data);
    } catch (error) {
      console.error('Error fetching discussions:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleCreateDiscussion = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newDiscussion.title || !newDiscussion.content) {
      toast.error('Please fill in all required fields');
      return;
    }

    setIsSubmitting(true);
    try {
      await discussionsApi.create(newDiscussion);
      toast.success('Discussion created!');
      setShowNewForm(false);
      setNewDiscussion({ title: '', content: '', category: 'General' });
      fetchDiscussions();
    } catch {
      toast.error('Failed to create discussion');
    } finally {
      setIsSubmitting(false);
    }
  };

  const categories = ['General', 'Career Advice', 'Technical', 'Job Search', 'Courses', 'Other'];

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <div className="h-12 w-12 animate-spin rounded-full border-4 border-primary-200 border-t-primary-600"></div>
      </div>
    );
  }

  return (
    <div>
      <div className="mb-8 flex justify-between items-start">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Discussions</h1>
          <p className="mt-1 text-sm text-gray-500">
            Ask questions, share knowledge, and connect with the community.
          </p>
        </div>
        <button
          onClick={() => setShowNewForm(!showNewForm)}
          className="inline-flex items-center px-4 py-2 bg-primary-600 text-white rounded-md hover:bg-primary-700"
        >
          <PlusIcon className="h-5 w-5 mr-2" />
          New Question
        </button>
      </div>

      {/* New Discussion Form */}
      {showNewForm && (
        <div className="bg-white rounded-lg shadow p-6 mb-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Ask a Question</h2>
          <form onSubmit={handleCreateDiscussion} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Title *
              </label>
              <input
                type="text"
                value={newDiscussion.title}
                onChange={(e) =>
                  setNewDiscussion({ ...newDiscussion, title: e.target.value })
                }
                placeholder="What's your question?"
                className="block w-full rounded-md border-0 py-2 px-3 text-gray-900 ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-inset focus:ring-primary-600"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Category
              </label>
              <select
                value={newDiscussion.category}
                onChange={(e) =>
                  setNewDiscussion({ ...newDiscussion, category: e.target.value })
                }
                className="block w-full rounded-md border-0 py-2 px-3 text-gray-900 ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-inset focus:ring-primary-600"
              >
                {categories.map((cat) => (
                  <option key={cat} value={cat}>
                    {cat}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Details *
              </label>
              <textarea
                value={newDiscussion.content}
                onChange={(e) =>
                  setNewDiscussion({ ...newDiscussion, content: e.target.value })
                }
                placeholder="Provide more details about your question..."
                rows={4}
                className="block w-full rounded-md border-0 py-2 px-3 text-gray-900 ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-inset focus:ring-primary-600"
              />
            </div>
            <div className="flex gap-3">
              <button
                type="submit"
                disabled={isSubmitting}
                className="px-4 py-2 bg-primary-600 text-white rounded-md hover:bg-primary-700 disabled:opacity-50"
              >
                {isSubmitting ? 'Posting...' : 'Post Question'}
              </button>
              <button
                type="button"
                onClick={() => setShowNewForm(false)}
                className="px-4 py-2 border border-gray-300 text-gray-700 rounded-md hover:bg-gray-50"
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

      {/* Filter */}
      <div className="mb-6">
        <select
          value={categoryFilter}
          onChange={(e) => setCategoryFilter(e.target.value)}
          className="rounded-md border-0 py-2 pl-3 pr-10 text-gray-900 ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-inset focus:ring-primary-600"
        >
          <option value="">All Categories</option>
          {categories.map((cat) => (
            <option key={cat} value={cat}>
              {cat}
            </option>
          ))}
        </select>
      </div>

      {/* Discussions List */}
      {discussions.length === 0 ? (
        <p className="text-gray-500">No discussions found. Be the first to ask a question!</p>
      ) : (
        <div className="space-y-4">
          {discussions.map((discussion) => (
            <Link
              key={discussion.id}
              to={`/discussions/${discussion.id}`}
              className="block bg-white rounded-lg shadow p-6 hover:shadow-md transition-shadow"
            >
              <div className="flex items-start justify-between">
                <div className="flex-1">
                  <div className="flex items-center gap-2 mb-1">
                    {discussion.isPinned && (
                      <span className="bg-yellow-100 text-yellow-800 text-xs px-2 py-0.5 rounded">
                        Pinned
                      </span>
                    )}
                    {discussion.isClosed && (
                      <span className="bg-gray-100 text-gray-800 text-xs px-2 py-0.5 rounded">
                        Closed
                      </span>
                    )}
                  </div>
                  <h3 className="text-lg font-semibold text-gray-900 hover:text-primary-600">
                    {discussion.title}
                  </h3>
                  <p className="mt-1 text-sm text-gray-600 line-clamp-2">
                    {discussion.content}
                  </p>
                  <div className="mt-3 flex items-center gap-4 text-sm text-gray-500">
                    <span className="flex items-center">
                      <TagIcon className="h-4 w-4 mr-1" />
                      {discussion.category}
                    </span>
                    <span className="flex items-center">
                      <ChatBubbleLeftIcon className="h-4 w-4 mr-1" />
                      {discussion.replyCount} replies
                    </span>
                    <span className="flex items-center">
                      <EyeIcon className="h-4 w-4 mr-1" />
                      {discussion.viewCount} views
                    </span>
                  </div>
                </div>
              </div>
              <div className="mt-4 flex items-center text-sm text-gray-500">
                <span>by {discussion.userName}</span>
                <span className="mx-2">·</span>
                <span>{format(new Date(discussion.createdAt), 'MMM d, yyyy')}</span>
              </div>
            </Link>
          ))}
        </div>
      )}
    </div>
  );
}
