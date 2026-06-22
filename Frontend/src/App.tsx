import { BrowserRouter, Routes, Route } from "react-router-dom";
import Layout from './layout/Layout.tsx'
import Dashboard from './pages/Dashboard/Dashboard.tsx'
import Trucks from './pages/Trucks/Trucks.tsx'
import Trips from "./pages/Trips/Trips.tsx";


export default function App() {
  return (
    <BrowserRouter>
      <Layout>
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/camiones" element={<Trucks />} />
          <Route path="/viajes" element={<Trips />} />
        </Routes>
      </Layout>
    </BrowserRouter>
  )
}
