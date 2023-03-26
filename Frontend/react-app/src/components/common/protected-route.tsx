import { Navigate } from 'react-router-dom'
import { useAuth } from '../../context/auth-context'

export default function ProtectedRoute({ children }: { children: JSX.Element }) {
  const { user } = useAuth()

  if (!user?.isAuthenticated) {
    return <Navigate to="/forbidden" replace state={{ fromProtected: true }} />
  }

  return children
}
