import { createBrowserRouter } from 'react-router-dom'
import App from './app'
import ErrorPage from './Pages/error-page'
import HomePage from './Pages/home-page'
import NotFoundPage from './Pages/not-found-page'

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
        path: '/create',
        element: <div>Create page</div>
      },
      {
        path: '/view',
        element: <div>View page</div>
      }
    ]
  },
  {
    path: '*',
    element: <NotFoundPage />
  }
])
