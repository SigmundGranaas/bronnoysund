import React, { type ReactNode } from 'react';

interface LayoutProps {
    children: ReactNode;
}

// Not really used anymore, as the layout was discarded.
const Layout: React.FC<LayoutProps> = ({ children }) => {
    return (
        <div className="flex h-screen bg-gray-100 text-gray-800">
            <main className={`flex-1 p-6 pt-8 overflow-y-auto`}>
                {children}
            </main>
        </div>
    );
};

export default Layout;