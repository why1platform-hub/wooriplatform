import { Fragment, useState } from 'react';
import { Link, Outlet, useNavigate } from 'react-router-dom';
import { Dialog, Menu, Transition } from '@headlessui/react';
import {
  Bars3Icon,
  HomeIcon,
  BookOpenIcon,
  UserGroupIcon,
  ChatBubbleLeftRightIcon,
  QuestionMarkCircleIcon,
  BriefcaseIcon,
  AcademicCapIcon,
  MegaphoneIcon,
  UserCircleIcon,
  Cog6ToothIcon,
  ArrowRightOnRectangleIcon,
} from '@heroicons/react/24/outline';
import { useAuth } from '../context/AuthContext';

function classNames(...classes: string[]) {
  return classes.filter(Boolean).join(' ');
}

export default function Layout() {
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const navigation = [
    { name: 'Dashboard', href: '/', icon: HomeIcon },
    { name: 'Courses', href: '/courses', icon: BookOpenIcon },
    { name: 'Programs', href: '/programs', icon: AcademicCapIcon },
    { name: 'Jobs', href: '/jobs', icon: BriefcaseIcon },
    { name: 'Consultants', href: '/consultants', icon: UserGroupIcon },
    { name: 'Discussions', href: '/discussions', icon: ChatBubbleLeftRightIcon },
    { name: 'FAQ', href: '/faq', icon: QuestionMarkCircleIcon },
    { name: 'Announcements', href: '/announcements', icon: MegaphoneIcon },
  ];

  const instructorNavigation = [
    { name: 'My Courses', href: '/instructor/courses', icon: BookOpenIcon },
    { name: 'Consultations', href: '/instructor/consultations', icon: UserGroupIcon },
  ];

  const adminNavigation = [
    { name: 'Manage Users', href: '/admin/users', icon: UserGroupIcon },
    { name: 'Manage Courses', href: '/admin/courses', icon: BookOpenIcon },
    { name: 'Manage Programs', href: '/admin/programs', icon: AcademicCapIcon },
    { name: 'Manage Jobs', href: '/admin/jobs', icon: BriefcaseIcon },
    { name: 'Manage Announcements', href: '/admin/announcements', icon: MegaphoneIcon },
    { name: 'Manage FAQ', href: '/admin/faq', icon: QuestionMarkCircleIcon },
  ];

  const isInstructor = user?.role === 'Instructor' || user?.role === 'Admin';
  const isAdmin = user?.role === 'Admin';

  return (
    <div>
      <Transition.Root show={sidebarOpen} as={Fragment}>
        <Dialog as="div" className="relative z-50 lg:hidden" onClose={setSidebarOpen}>
          <Transition.Child
            as={Fragment}
            enter="transition-opacity ease-linear duration-300"
            enterFrom="opacity-0"
            enterTo="opacity-100"
            leave="transition-opacity ease-linear duration-300"
            leaveFrom="opacity-100"
            leaveTo="opacity-0"
          >
            <div className="fixed inset-0 bg-gray-900/80" />
          </Transition.Child>

          <div className="fixed inset-0 flex">
            <Transition.Child
              as={Fragment}
              enter="transition ease-in-out duration-300 transform"
              enterFrom="-translate-x-full"
              enterTo="translate-x-0"
              leave="transition ease-in-out duration-300 transform"
              leaveFrom="translate-x-0"
              leaveTo="-translate-x-full"
            >
              <Dialog.Panel className="relative mr-16 flex w-full max-w-xs flex-1">
                <div className="flex grow flex-col gap-y-5 overflow-y-auto bg-primary-700 px-6 pb-4">
                  <div className="flex h-16 shrink-0 items-center">
                    <span className="text-2xl font-bold text-white">Woori LMS</span>
                  </div>
                  <nav className="flex flex-1 flex-col">
                    <ul role="list" className="flex flex-1 flex-col gap-y-7">
                      <li>
                        <ul role="list" className="-mx-2 space-y-1">
                          {navigation.map((item) => (
                            <li key={item.name}>
                              <Link
                                to={item.href}
                                className="group flex gap-x-3 rounded-md p-2 text-sm font-semibold leading-6 text-primary-100 hover:bg-primary-600 hover:text-white"
                                onClick={() => setSidebarOpen(false)}
                              >
                                <item.icon className="h-6 w-6 shrink-0" aria-hidden="true" />
                                {item.name}
                              </Link>
                            </li>
                          ))}
                        </ul>
                      </li>
                      {isInstructor && (
                        <li>
                          <div className="text-xs font-semibold leading-6 text-primary-200">Instructor</div>
                          <ul role="list" className="-mx-2 mt-2 space-y-1">
                            {instructorNavigation.map((item) => (
                              <li key={item.name}>
                                <Link
                                  to={item.href}
                                  className="group flex gap-x-3 rounded-md p-2 text-sm font-semibold leading-6 text-primary-100 hover:bg-primary-600 hover:text-white"
                                  onClick={() => setSidebarOpen(false)}
                                >
                                  <item.icon className="h-6 w-6 shrink-0" aria-hidden="true" />
                                  {item.name}
                                </Link>
                              </li>
                            ))}
                          </ul>
                        </li>
                      )}
                      {isAdmin && (
                        <li>
                          <div className="text-xs font-semibold leading-6 text-primary-200">Admin</div>
                          <ul role="list" className="-mx-2 mt-2 space-y-1">
                            {adminNavigation.map((item) => (
                              <li key={item.name}>
                                <Link
                                  to={item.href}
                                  className="group flex gap-x-3 rounded-md p-2 text-sm font-semibold leading-6 text-primary-100 hover:bg-primary-600 hover:text-white"
                                  onClick={() => setSidebarOpen(false)}
                                >
                                  <item.icon className="h-6 w-6 shrink-0" aria-hidden="true" />
                                  {item.name}
                                </Link>
                              </li>
                            ))}
                          </ul>
                        </li>
                      )}
                    </ul>
                  </nav>
                </div>
              </Dialog.Panel>
            </Transition.Child>
          </div>
        </Dialog>
      </Transition.Root>

      {/* Static sidebar for desktop */}
      <div className="hidden lg:fixed lg:inset-y-0 lg:z-50 lg:flex lg:w-72 lg:flex-col">
        <div className="flex grow flex-col gap-y-5 overflow-y-auto bg-primary-700 px-6 pb-4">
          <div className="flex h-16 shrink-0 items-center">
            <span className="text-2xl font-bold text-white">Woori LMS</span>
          </div>
          <nav className="flex flex-1 flex-col">
            <ul role="list" className="flex flex-1 flex-col gap-y-7">
              <li>
                <ul role="list" className="-mx-2 space-y-1">
                  {navigation.map((item) => (
                    <li key={item.name}>
                      <Link
                        to={item.href}
                        className="group flex gap-x-3 rounded-md p-2 text-sm font-semibold leading-6 text-primary-100 hover:bg-primary-600 hover:text-white"
                      >
                        <item.icon className="h-6 w-6 shrink-0" aria-hidden="true" />
                        {item.name}
                      </Link>
                    </li>
                  ))}
                </ul>
              </li>
              {isInstructor && (
                <li>
                  <div className="text-xs font-semibold leading-6 text-primary-200">Instructor</div>
                  <ul role="list" className="-mx-2 mt-2 space-y-1">
                    {instructorNavigation.map((item) => (
                      <li key={item.name}>
                        <Link
                          to={item.href}
                          className="group flex gap-x-3 rounded-md p-2 text-sm font-semibold leading-6 text-primary-100 hover:bg-primary-600 hover:text-white"
                        >
                          <item.icon className="h-6 w-6 shrink-0" aria-hidden="true" />
                          {item.name}
                        </Link>
                      </li>
                    ))}
                  </ul>
                </li>
              )}
              {isAdmin && (
                <li>
                  <div className="text-xs font-semibold leading-6 text-primary-200">Admin</div>
                  <ul role="list" className="-mx-2 mt-2 space-y-1">
                    {adminNavigation.map((item) => (
                      <li key={item.name}>
                        <Link
                          to={item.href}
                          className="group flex gap-x-3 rounded-md p-2 text-sm font-semibold leading-6 text-primary-100 hover:bg-primary-600 hover:text-white"
                        >
                          <item.icon className="h-6 w-6 shrink-0" aria-hidden="true" />
                          {item.name}
                        </Link>
                      </li>
                    ))}
                  </ul>
                </li>
              )}
            </ul>
          </nav>
        </div>
      </div>

      <div className="lg:pl-72">
        <div className="sticky top-0 z-40 flex h-16 shrink-0 items-center gap-x-4 border-b border-gray-200 bg-white px-4 shadow-sm sm:gap-x-6 sm:px-6 lg:px-8">
          <button
            type="button"
            className="-m-2.5 p-2.5 text-gray-700 lg:hidden"
            onClick={() => setSidebarOpen(true)}
          >
            <span className="sr-only">Open sidebar</span>
            <Bars3Icon className="h-6 w-6" aria-hidden="true" />
          </button>

          <div className="h-6 w-px bg-gray-900/10 lg:hidden" aria-hidden="true" />

          <div className="flex flex-1 gap-x-4 self-stretch lg:gap-x-6">
            <div className="flex flex-1"></div>
            <div className="flex items-center gap-x-4 lg:gap-x-6">
              <Menu as="div" className="relative">
                <Menu.Button className="-m-1.5 flex items-center p-1.5">
                  <span className="sr-only">Open user menu</span>
                  {user?.profileImageUrl ? (
                    <img
                      className="h-8 w-8 rounded-full bg-gray-50"
                      src={user.profileImageUrl}
                      alt=""
                    />
                  ) : (
                    <UserCircleIcon className="h-8 w-8 text-gray-400" />
                  )}
                  <span className="hidden lg:flex lg:items-center">
                    <span
                      className="ml-4 text-sm font-semibold leading-6 text-gray-900"
                      aria-hidden="true"
                    >
                      {user?.firstName} {user?.lastName}
                    </span>
                  </span>
                </Menu.Button>
                <Transition
                  as={Fragment}
                  enter="transition ease-out duration-100"
                  enterFrom="transform opacity-0 scale-95"
                  enterTo="transform opacity-100 scale-100"
                  leave="transition ease-in duration-75"
                  leaveFrom="transform opacity-100 scale-100"
                  leaveTo="transform opacity-0 scale-95"
                >
                  <Menu.Items className="absolute right-0 z-10 mt-2.5 w-48 origin-top-right rounded-md bg-white py-2 shadow-lg ring-1 ring-gray-900/5 focus:outline-none">
                    <Menu.Item>
                      {({ active }) => (
                        <Link
                          to="/profile"
                          className={classNames(
                            active ? 'bg-gray-50' : '',
                            'flex items-center px-3 py-1 text-sm leading-6 text-gray-900'
                          )}
                        >
                          <UserCircleIcon className="mr-2 h-5 w-5" />
                          My Profile
                        </Link>
                      )}
                    </Menu.Item>
                    <Menu.Item>
                      {({ active }) => (
                        <Link
                          to="/settings"
                          className={classNames(
                            active ? 'bg-gray-50' : '',
                            'flex items-center px-3 py-1 text-sm leading-6 text-gray-900'
                          )}
                        >
                          <Cog6ToothIcon className="mr-2 h-5 w-5" />
                          Settings
                        </Link>
                      )}
                    </Menu.Item>
                    <Menu.Item>
                      {({ active }) => (
                        <button
                          onClick={handleLogout}
                          className={classNames(
                            active ? 'bg-gray-50' : '',
                            'flex w-full items-center px-3 py-1 text-sm leading-6 text-gray-900'
                          )}
                        >
                          <ArrowRightOnRectangleIcon className="mr-2 h-5 w-5" />
                          Sign out
                        </button>
                      )}
                    </Menu.Item>
                  </Menu.Items>
                </Transition>
              </Menu>
            </div>
          </div>
        </div>

        <main className="py-10">
          <div className="px-4 sm:px-6 lg:px-8">
            <Outlet />
          </div>
        </main>
      </div>
    </div>
  );
}
