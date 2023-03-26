import classNames from 'classnames'
import { useState } from 'react'
import { Outlet } from 'react-router-dom'
import Header from './components/common/header'
import SideNav from './components/common/side-nav'
import { useAuth } from './context/auth-context'
import { useInitialLoad } from './hooks/use-initial-load'

function App() {
  const [showSideNav, setShowSideNav] = useState(true)
  const toggleSideNav = () => {
    setShowSideNav(!showSideNav)
  }
  const { user, userFetched } = useAuth()

  const { isFetching } = useInitialLoad()

  if (isFetching || !userFetched) return <div>Loading</div>
  return (
    <>
      <section className="fixed top-0 right-0 left-0 h-14 bg-blue-500">
        <Header />
      </section>
      <section className=" flex h-screen flex-nowrap pt-14">
        {user?.isAuthenticated ? (
          <aside className={classNames(showSideNav ? 'w-60' : 'w-9', 'max-w-full border-x-2')}>
            <SideNav toggleSideNav={toggleSideNav} showSideNav={showSideNav} />
          </aside>
        ) : null}

        <main className="max-w-full grow basis-0">
          <Outlet />
        </main>
      </section>
    </>
  )
}

export default App
