import { useRouteError } from 'react-router-dom'

export default function ErrorPage() {
  const error = useRouteError() as any
  console.error(error)

  return (
    <section className="container flex justify-center">
      <div>
        <h1 className="text-xl font-bold">Oops!</h1>
        <p>Sorry, an unexpected error has occurred.</p>
        <p>
          <i>{error?.statusText || error?.message}</i>
        </p>
      </div>
    </section>
  )
}
