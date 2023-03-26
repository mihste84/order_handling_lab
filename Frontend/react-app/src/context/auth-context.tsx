import axios from 'axios'
import React, { useState, useEffect, useContext, ReactElement } from 'react'

export interface IAuthContext {
  user?: AppUser
  userFetched?: boolean
}

export interface AppUser {
  isAuthenticated: boolean
  userName?: string
}

export const AuthContext = React.createContext<IAuthContext>({})
export const useAuth = () => useContext(AuthContext)
let startFetch = false

export const AuthProvider = ({ children }: { children: ReactElement }) => {
  const [user, setUser] = useState<AppUser>({ isAuthenticated: false })
  const [userFetched, setUserFetched] = useState(false)
  const getAppUser = async () => {
    startFetch = true
    const { data } = await axios.get<AppUser>(import.meta.env.VITE_API_ENDPOINT + 'auth/getAppUser', {
      withCredentials: true
    })

    setUserFetched(true)
    setUser(data)
  }

  useEffect(() => {
    if (!startFetch) getAppUser()
  }, [])

  return (
    <AuthContext.Provider
      value={{
        user,
        userFetched
      }}>
      {children}
    </AuthContext.Provider>
  )
}
