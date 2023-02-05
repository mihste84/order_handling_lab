import classNames from 'classnames'
import { ReactNode, useState } from 'react'

export default function Dropdown({
  className,
  isOpen,
  button,
  children,
  onBackdropClick
}: {
  button: ReactNode
  isOpen: boolean
  className?: string
  children?: ReactNode[]
  onBackdropClick?: () => void
}) {
  const items = children ? children : <div className="whitespace-nowrap px-4 py-2">No items to display</div>
  const dropdown = isOpen ? (
    <section className={classNames(className, 'absolute z-40')}>
      <section className="divide-y rounded-sm border bg-white shadow-md ">{items}</section>
    </section>
  ) : null
  const backdrop =
    onBackdropClick && isOpen ? (
      <div
        className="fixed top-0 right-0 left-0 bottom-0 z-30 cursor-default bg-transparent"
        onClick={onBackdropClick}></div>
    ) : null
  return (
    <section>
      {backdrop}
      {button}
      {dropdown}
    </section>
  )
}
