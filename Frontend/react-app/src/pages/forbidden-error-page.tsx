import { Navigate, useLocation } from 'react-router-dom'

export default function ForbiddenErrorPage() {
  const location = useLocation()
  const fromProtected: { fromProtected?: boolean } = location.state?.fromProtected

  if (!fromProtected) {
    return <Navigate to="/" replace />
  }

  return (
    <section className="container flex justify-center">
      <div>
        <h1 className="text-xl font-bold">Auth error!</h1>
        <p>Sorry, you are not allowed to access this page.</p>
      </div>
    </section>
  )
}
