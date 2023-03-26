import { createBrowserRouter } from 'react-router-dom'
import App from './app'
import ProtectedRoute from './components/common/protected-route'
import ErrorPage from './pages/error-page'
import ForbiddenErrorPage from './pages/forbidden-error-page'
import HomePage from './pages/home-page'
import MonitorPage from './pages/monitor-page'
import NotFoundPage from './pages/not-found-page'
import WorkflowsPage from './pages/workflows-page'

export default createBrowserRouter([
  {
    path: '/',
    element: <App />,
    errorElement: <ErrorPage />,
    children: [
      {
        path: '/',
        element: <HomePage />
      },
      {
        path: '/orders',
        element: <ProtectedRoute children={<WorkflowsPage />} />
      },
      {
        path: '/customers',
        element: <ProtectedRoute children={<WorkflowsPage />} />
      },
      {
        path: '/masterdata',
        element: <ProtectedRoute children={<WorkflowsPage />} />
      },
      {
        path: '/monitor',
        element: <ProtectedRoute children={<MonitorPage />} />
      },
      {
        path: '/login',
        element: <LoginPage />
      },
      {
        path: '/logout',
        element: <ProtectedRoute children={<LogoutPage />} />
      }
    ]
  },
  {
    path: '/forbidden',
    element: <ForbiddenErrorPage />
  },
  {
    path: '*',
    element: <NotFoundPage />
  }
])

function LoginPage() {
  window.location.href = import.meta.env.VITE_API_ENDPOINT + 'auth/login'
  return null
}

function LogoutPage() {
  window.location.href = import.meta.env.VITE_API_ENDPOINT + 'auth/logout'
  return null
}
