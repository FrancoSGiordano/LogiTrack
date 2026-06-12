import type React from "react"
import { useState } from "react";
import { Link } from 'react-router-dom';
import { LayoutDashboard, Truck, Map, Menu, X, TowerControl } from "lucide-react";
import styles from './Layout.module.css';

type LayoutProps = {
    children : React.ReactNode
}

export default function Layout({ children } : LayoutProps) {

    const [isSidebarOpen, setIsSidebarOpen] = useState(false);

    const toggleSidebar = () => setIsSidebarOpen(!isSidebarOpen);
    const closeSidebar = () => setIsSidebarOpen(false);


    return (
        <div className={styles.container}>
            <div 
                className={`${styles.overlay} ${isSidebarOpen ? styles.overlayOpen : ''}`}
                onClick={closeSidebar}
            />

            <aside className={`${styles.sidebar} ${isSidebarOpen ? styles.sidebarOpen : ''}`}>
                <div className={styles.brand}>
                    <div className={styles.brandContent}>
                        <LayoutDashboard size={24} />
                        <span>LogiTrack</span>
                    </div>
                    <button 
                        className={styles.closeButton}
                        onClick={closeSidebar}
                        aria-label="Cerrar menú"
                    >
                        <X size={24}/>
                    </button>
                </div>

                <nav className={styles.nav}>
                    <Link 
                        to="/"
                        className={styles.navLink}
                        onClick={closeSidebar}
                    >
                        <TowerControl size={20}/> Torre de Control
                    </Link>
                    <Link
                        to="/camiones"
                        className={styles.navLink}
                        onClick={closeSidebar}
                    >
                        <Truck size={20}/> Gestión de Flota
                    </Link>
                    <Link
                        to="/viajes"
                        className={styles.navLink}
                        onClick={closeSidebar}
                    >
                        <Map size={20}/> Asignar Viajes
                    </Link>
                </nav>
            </aside>

            <div className={styles.mainWrapper}>
                <header className={styles.mobileHeader}>
                    <span className={styles.mobileTitle}>LogiTrack</span>
                    <button
                        className={styles.menuButton}
                        onClick={toggleSidebar}
                        aria-label="Abrir menú"
                    >
                        <Menu size={24}/>
                    </button>
                </header>

                <main className={styles.mainContent}>
                    {children}
                </main>
            </div>
        </div>
    )
}
