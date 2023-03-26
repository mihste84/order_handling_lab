import { NavLink } from 'react-router-dom'
import { HomeIcon, EyeIcon, UsersIcon, DocumentTextIcon, CircleStackIcon } from '@heroicons/react/24/outline'

function NavLinkWrapper({
  linkText,
  path,
  icon,
  showSideNav
}: {
  linkText: string
  path: string
  icon: JSX.Element
  showSideNav: boolean
}) {
  const className = showSideNav ? 'w-8 pr-2' : 'w-8 pr-0'
  const text = showSideNav ? <span>{linkText}</span> : null

  return (
    <NavLink to={path} className={({ isActive }) => (isActive ? 'pointer-events-none opacity-50' : 'hover:opacity-70')}>
      <div className="flex items-center justify-start">
        <span className={className}>{icon}</span>
        {text}
      </div>
    </NavLink>
  )
}

export default function NavLinks({ showSideNav }: { showSideNav: boolean }) {
  const className = showSideNav ? 'py-2 px-4' : 'py-2 px-1'

  return (
    <nav>
      <ul>
        <li className={className}>
          <NavLinkWrapper showSideNav={showSideNav} linkText="Home" path="/" icon={<HomeIcon className="" />} />
        </li>
        <li className={className}>
          <NavLinkWrapper
            showSideNav={showSideNav}
            linkText="Orders & Invoices"
            path="/orders"
            icon={<DocumentTextIcon />}
          />
        </li>
        <li className={className}>
          <NavLinkWrapper showSideNav={showSideNav} linkText="Customers" path="/customers" icon={<UsersIcon />} />
        </li>
        <li className={className}>
          <NavLinkWrapper
            showSideNav={showSideNav}
            linkText="Master data"
            path="/masterdata"
            icon={<CircleStackIcon />}
          />
        </li>
        <li className={className}>
          <NavLinkWrapper showSideNav={showSideNav} linkText="Monitor" path="/monitor" icon={<EyeIcon />} />
        </li>
      </ul>
    </nav>
  )
}
