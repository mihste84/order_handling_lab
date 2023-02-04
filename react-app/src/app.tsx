import { Outlet } from 'react-router-dom'
import SideNav from './components/common/side-nav'

function App() {
  return (
    <div className="container flex h-screen flex-nowrap ">
      <SideNav />
      <main className="max-w-full grow basis-0">
        <Outlet />
      </main>
    </div>
  )
}

export default App
