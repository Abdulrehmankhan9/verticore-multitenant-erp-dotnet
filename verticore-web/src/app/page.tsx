import Link from "next/link";
import styles from "./page.module.css";

export default function HomePage() {
  return (
    <main className={styles.page}>
      <header className={styles.header}>
        <Link className={styles.brand} href="/" aria-label="VertiCore home">
          <span className={styles.brandMark}>V</span>
          <span className={styles.brandText}>
            <span>ERP</span>
            <strong>VertiCore</strong>
          </span>
        </Link>
        <nav className={styles.headerNav} aria-label="Main navigation">
          <a href="#platform">Platform</a>
          <a href="#workflow">Workflow</a>
        </nav>
        <div className={styles.headerActions}>
          <Link className={styles.loginLink} href="/login">Log in</Link>
          <Link className={styles.headerCta} href="/register">Create workspace</Link>
        </div>
      </header>

      <section className={styles.hero} id="platform">
        <div className={styles.heroCopy}>
          <p className={styles.eyebrow}><span /> Operations, brought together</p>
          <h1>Make the work behind your business <em>work better.</em></h1>
          <p className={styles.intro}>
            Keep clients, invoices, and your team in one clear workspace, so the next step is always easy to see.
          </p>
          <div className={styles.heroActions}>
            <Link className={styles.primaryCta} href="/register">Create your workspace <span aria-hidden="true">↗</span></Link>
            <Link className={styles.secondaryCta} href="/login">Sign in to VertiCore</Link>
          </div>
          <p className={styles.heroNote}>A focused home for the day-to-day of your business.</p>
        </div>

        <div className={styles.previewWrap} aria-label="Preview of the VertiCore business dashboard">
          <div className={styles.previewAccent} />
          <div className={styles.preview}>
            <div className={styles.previewBar}>
              <div className={styles.previewBrand}><span>V</span> VERTICORE</div>
              <div className={styles.previewStatus}><i /> Workspace preview</div>
            </div>
            <div className={styles.previewBody}>
              <aside className={styles.previewSidebar}>
                <span className={styles.sidebarLabel}>WORKSPACE</span>
                <span className={styles.sidebarActive}>Overview</span>
                <span>Clients</span>
                <span>Invoices</span>
                <span>Team</span>
                <div className={styles.previewProfile}><b>AC</b><span>Acme Co.<small>Business account</small></span></div>
              </aside>
              <div className={styles.previewMain}>
                <div className={styles.previewHeading}><span>OVERVIEW</span><strong>Business at a glance</strong></div>
                <div className={styles.previewStats}>
                  <div><span>Collected</span><b>$24,850</b><small className={styles.positive}>↑ 12.8%</small></div>
                  <div><span>Awaiting</span><b>$6,240</b><small>4 invoices</small></div>
                  <div><span>Active clients</span><b>18</b><small>Across your workspace</small></div>
                </div>
                <div className={styles.previewTable}>
                  <div className={styles.tableTitle}><strong>Recent invoices</strong><span>View all</span></div>
                  <div className={styles.tableHead}><span>CLIENT</span><span>INVOICE</span><span>STATUS</span><span>AMOUNT</span></div>
                  <div className={styles.tableRow}><span><b>Northstar Studio</b><small>Design services</small></span><span>INV-0248</span><i>Paid</i><strong>$2,400</strong></div>
                  <div className={styles.tableRow}><span><b>Fieldwork Supply</b><small>Monthly retainer</small></span><span>INV-0247</span><i className={styles.pending}>Pending</i><strong>$1,850</strong></div>
                  <div className={styles.tableRow}><span><b>Common Ground</b><small>Consulting</small></span><span>INV-0246</span><i>Paid</i><strong>$960</strong></div>
                </div>
              </div>
            </div>
          </div>
          <div className={styles.previewCaption}><span>01 / BUSINESS OVERVIEW</span><span>Less chasing. More clarity.</span></div>
        </div>
      </section>

      <section className={styles.workflow} id="workflow">
        <div className={styles.workflowIntro}>
          <p className={styles.eyebrow}>One connected workspace</p>
          <h2>From first client to final payment.</h2>
        </div>
        <div className={styles.workflowItems}>
          <article><span>01</span><h3>Know your clients</h3><p>Keep contact details and account activity together.</p></article>
          <article><span>02</span><h3>Send clear invoices</h3><p>Build itemized invoices and track what is outstanding.</p></article>
          <article><span>03</span><h3>See the whole picture</h3><p>Follow revenue, overdue work, and team activity from one dashboard.</p></article>
        </div>
      </section>

      <footer className={styles.footer}>
        <span>VertiCore <span aria-hidden="true">/</span> Business operations, made clearer.</span>
        <Link href="/register">Get started <span aria-hidden="true">↗</span></Link>
      </footer>
    </main>
  );
}
