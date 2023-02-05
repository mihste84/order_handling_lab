import { BellIcon, ExclamationTriangleIcon } from '@heroicons/react/24/solid'
import { useState } from 'react'
import AlertItem, { AlertType } from '../dropdown/alert-item'
import Dropdown from '../dropdown/dropdown'

export default function Header() {
  const [alertOpen, setAlertOpen] = useState(false)
  const onDismissAlert = (id: number) => {}
  let items = [
    <AlertItem
      createDate={new Date()}
      message="Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut euismod facilisis erat. Pellentesque felis arcu, venenatis et volutpat id, convallis in elit. Etiam erat felis, malesuada in volutpat in, rhoncus in lacus. Donec laoreet est a enim blandit, eget luctus dolor porta. Nulla semper id dui ac blandit. Fusce hendrerit id est quis porta. Maecenas rhoncus diam ipsum, sed scelerisque nibh vehicula et. Donec cursus posuere ligula. Nulla sollicitudin ex aliquam finibus porttitor. Pellentesque tempus sit amet dui a porta. Etiam urna arcu, ultricies sed pretium nec, imperdiet vel lorem. Nunc efficitur urna aliquam, euismod velit nec, efficitur leo. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Maecenas ultrices arcu sem, vitae venenatis libero porttitor a."
      title="Title asd as asd asdasdasasd asd asd"
      key="1"
      id={1}
      onDismissAlert={onDismissAlert}
      type={AlertType.info}
    />,
    <AlertItem
      createDate={new Date()}
      message="Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut euismod facilisis erat. "
      title="Title"
      key="2"
      id={2}
      onDismissAlert={onDismissAlert}
      type={AlertType.error}
    />
  ]

  return (
    <header className="flex h-full items-center justify-start px-4 text-white">
      <h1 className="text-xl">{import.meta.env.VITE_APP_TITLE}</h1>
      <section className="flex-auto"></section>
      <section className="relative cursor-pointer text-black">
        <Dropdown
          isOpen={alertOpen}
          className="right-0"
          children={items}
          onBackdropClick={() => setAlertOpen(false)}
          button={<BellIcon onClick={() => setAlertOpen(!alertOpen)} className="w-7 text-white" />}
        />
      </section>
    </header>
  )
}
