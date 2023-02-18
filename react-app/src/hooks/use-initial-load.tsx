import { useQuery } from 'react-query'
import { WaitActivityMetadata } from '../types/activity-meta-data'
import { request, gql } from 'graphql-request'

const query = gql`
  {
    waitActivityMetadata {
      name
      description
      input {
        name
        type
        validators {
          required {
            message
          }
          maxLength {
            message
            maxLength
          }
          minLength {
            message
            minLength
          }
        }
      }
    }
  }
`

export function useInitialLoad() {
  const fetchInitial = async () => {
    return await request(import.meta.env.VITE_API_ENDPOINT, query)
  }

  return useQuery<WaitActivityMetadata, Error>('metadata', fetchInitial)
}
