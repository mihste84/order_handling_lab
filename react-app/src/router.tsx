import { createBrowserRouter } from 'react-router-dom'
import App from './app'
import ErrorPage from './Pages/error-page'
import HomePage from './Pages/home-page'
import MonitorPage from './Pages/monitor-page'
import NotFoundPage from './Pages/not-found-page'
import WorkflowsPage from './Pages/workflows-page'

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
        path: '/workflows',
        element: <WorkflowsPage />
      },
      {
        path: '/monitor',
        element: <MonitorPage />
      }
    ]
  },
  {
    path: '*',
    element: <NotFoundPage />
  }
])
