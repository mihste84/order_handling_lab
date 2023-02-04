import NavLinks from './nav-links'

export default function SideNav() {
  return (
    <aside className="flex max-w-full flex-col">
      <section className="flex-auto">
        <NavLinks />
      </section>
      <div>Name</div>
    </aside>
  )
}
