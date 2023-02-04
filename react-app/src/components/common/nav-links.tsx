import { Link } from 'react-router-dom'

export default function NavLinks() {
  return (
    <>
      <nav>
        <ul>
          <li>
            <Link to={'/'} about="Home page">
              Home
            </Link>
          </li>
          <li>
            <Link to={'/view'} about="View workflows page">
              View workflows
            </Link>
          </li>
          <li>
            <Link to={'/create'} about="Create workflow page">
              Add new workflow
            </Link>
          </li>
        </ul>
      </nav>
    </>
  )
}
