import "../styles/footer.css";

export default function Footer() {
  return (
    <footer className="footer">
      <div className="footer-inner">
        <span>© 2025 WebStore</span>
     <span className="footer-tech">
  © {new Date().getFullYear()} Besarta Clothing · Crafted for style.
</span>
      </div>
    </footer>
  );
}
