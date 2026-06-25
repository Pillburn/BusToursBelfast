#!/bin/bash
# cleanup.sh

echo "🧹 Cleaning up unused files..."

# Files and folders to remove (adjust based on your project)

# 1. Remove duplicate folders (if any)
rm -rf src/components/tour/form 2>/dev/null
rm -rf src/features/tours/form 2>/dev/null

# 2. Remove unused components
rm -rf src/features/admin 2>/dev/null

# 3. Remove unused styles
rm -f src/index.css 2>/dev/null  # Only if you're using MUI exclusively

# 4. Remove unused assets
# rm -rf public/images/default-tour.jpg 2>/dev/null  # Only if you have default

# 5. Remove unused test files (if not using tests yet)
# rm -rf src/__tests__ 2>/dev/null

# 6. Remove backup files
find . -name "*.bak" -type f -delete
find . -name "*.backup" -type f -delete

# 7. Remove unnecessary config files (if any)
# rm -f .eslintrc.json 2>/dev/null  # Only if using different linter

echo "✅ Cleanup complete!"