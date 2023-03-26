import NavLinks from './nav-links'
import { ChevronDoubleLeftIcon, ChevronDoubleRightIcon } from '@heroicons/react/24/outline'

export default function SideNav({ toggleSideNav, showSideNav }: { toggleSideNav: () => void; showSideNav: boolean }) {
  const className = showSideNav ? 'py-2 px-4' : 'py-2 px-1'
  const iconClassName = showSideNav ? 'w-8 pr-2' : 'w-8 pr-0'
  const text = showSideNav ? <span>Toggle side menu</span> : null
  const icon = showSideNav ? <ChevronDoubleLeftIcon /> : <ChevronDoubleRightIcon />

  return (
    <section className="flex h-full flex-col">
      <section className="flex-auto">
        <NavLinks showSideNav={showSideNav} />
      </section>
      <section onClick={toggleSideNav}>
        <nav>
          <ul>
            <li className={className}>
              <div className="flex cursor-pointer items-center justify-start">
                <span className={iconClassName}>{icon}</span>
                {text}
              </div>
            </li>
          </ul>
        </nav>
      </section>
    </section>
  )
}
