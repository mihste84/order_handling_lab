import { XMarkIcon } from '@heroicons/react/24/outline'
import classNames from 'classnames'
import { format } from 'date-fns'

export enum AlertType {
  info,
  warning,
  error
}

export interface IAlertItem {
  id: number
  message: string
  title: string
  createDate: Date
  type?: AlertType
}

const getClassByType = (type?: AlertType) => {
  switch (type) {
    case AlertType.info:
      return 'border-blue-500'
    case AlertType.warning:
      return 'border-yellow-600'
    case AlertType.error:
      return 'border-red-600'
    default:
      return ''
  }
}

export default function AlertItem({
  onDismissAlert,
  ...props
}: {
  id: number
  message: string
  title: string
  createDate: Date
  type?: AlertType
  onDismissAlert: (id: number) => void
}) {
  return (
    <article
      className={classNames(
        'flex max-h-fit w-60 cursor-default flex-col items-stretch border-l-8 p-1',
        getClassByType(props.type)
      )}>
      <h4 className="mb-1 flex items-start text-sm">
        <span>{props.title}</span>
        <div className="flex-auto"></div>
        <XMarkIcon className="w-4 cursor-pointer hover:stroke-gray-500" onClick={() => onDismissAlert(props.id)} />
      </h4>
      <p className="flex-auto text-xs font-thin">{props.message}</p>
      <small className="text-right text-xs font-thin">{format(props.createDate, 'yyyy-MM-dd hh:mm:ss')}</small>
    </article>
  )
}
